#Const Amerocentric = True

Imports System.Runtime.InteropServices
Imports System
Imports System.Xml.Serialization
Imports System.Net
Imports System.Security.Cryptography.X509Certificates
Imports System.Security.Policy
Imports System.Net.Security
Imports System.Globalization
Imports System.Windows.Forms
Imports System.IO
Imports Starksoft.Net.Ftp

Namespace devX

#Region "    AutoUpgrade Class    "
    Public Class AutoUpgrade

#Region "    Constants    "
        Protected Const BUFFERSIZE As Int32 = 16384
        Protected Const SAVED_MANIFEST As String = "manifest.xml"
        Protected Const AUTOUPGRADE_XMLNS As String = "http://www.devx.com/schemas/autoupgrade/1.0"
        Protected Const LIBDLL_FILENAME As String = "AutoUpgrade.Lib.dll"
        Protected Const STUBEXE_FILENAME As String = "AutoUpgrade.exe"
#End Region

#Region "    Storage for Properties    "
        Private mblnCancel As Boolean = False
        Private mstrAppName As String = "OneStory Editor AutoUpdater"
        Private mstrSource As String = ""
        Private mlstManifest As New FileList()
        Private mlstUpgradeList As New FileList()
        Private mlngTotalSize As Long = 0
        Private mtimStartTime As System.DateTime
        Private mlngTotalBytesRead As Long = 0
        Private mstrFullUpgradeName As String = ""
        Private mtsOffsetToUCT As DateTimeOffset = New DateTimeOffset(DateTime.Now)
        Private mstrApplicationBasePath As String = ""
        Private mstrApplicationExecutable As String = ""
#End Region

#Region "    AutoUpgrade Class    "
        Public Event UpgradeProgress(ByVal strProgressString As String, ByVal strCurrentFile As String, ByVal intProgressPercent As Integer, ByVal lngTimeRemaining As Long, ByRef blnCancel As Boolean)

        Public Sub New()
            ' The XML serializer requires a default public constructor.  However, 
            ' users of this class should use one of the shared Create() methods unless
            ' you are using this class to generate a manifest, either manually (adding
            ' AutoUpgrade.File objects to the ManifestFiles collection) or by calling
            ' GenerateManifest
        End Sub

        Public Shared Function Create(ByVal strManifest As String, ByVal strSource As String) As AutoUpgrade
            ' Create and return a new instance of AutoUpgrade, setting the Application 
            ' and Source properties, and deserializing the object model from strManifest,
            ' which must be an XML string matching the schema of this class.
            ' This version of Create is designed for use when the manifest data is
            ' returned independantly of this class (for example, my Login webservice
            ' returns the manifest data on success in order to save a round-trip).
            Dim upgInstance As AutoUpgrade
            Dim xmlSerializer As XmlSerializer

            If strManifest Is Nothing OrElse strManifest.Length = 0 Then
                ' Manifest string is empty - return an empty AutoUpgrade object
                upgInstance = New AutoUpgrade()
                Return upgInstance
            Else
                Try
                    ' Read properties from manifest string passed in as argument
                    xmlSerializer = New XmlSerializer(GetType(AutoUpgrade), AUTOUPGRADE_XMLNS)
                    upgInstance = xmlSerializer.Deserialize(New IO.StringReader(strManifest))

                    ' We only set the SourcePath if the manifest file doesn't already 
                    ' contain one.  This way, if the host was a web service, or ASP page 
                    ' it could "redirect" clients to a different path for the 
                    ' application files by setting SourcePath in the xml manifest data
                    If upgInstance.SourcePath.Length = 0 Then
                        upgInstance.SourcePath = strSource
                    End If

                    ' If the manifest file did not contain an application base path (which it
                    ' should not, when retrieved from the server - it should contain one only
                    ' when AutoUpgrade.EXE is executed from StartUpgradeStub), set it to the
                    ' application's base directory
                    If upgInstance.ApplicationBasePath.Length = 0 Then
                        upgInstance.ApplicationBasePath = AppDomain.CurrentDomain.BaseDirectory
                    End If

                    xmlSerializer = Nothing
                    Return upgInstance

                Catch exc As Exception
                    Throw New System.Exception("Invalid Manifest", exc)
                End Try
            End If
        End Function

        Public Shared Function Create() As AutoUpgrade
            ' Create and return a new instance of AutoUpgrade, and deserialize the 
            ' object model from a file called manifest.xml in the same directory as
            ' this assembly.
            ' This version of create is for use by the AutoUpgrade.EXE stub, which
            ' is run via the StartUpgradeStub method.
            Dim xmlSerializer As New XmlSerializer(GetType(AutoUpgrade), AUTOUPGRADE_XMLNS)
            Dim manifestFile As IO.StreamReader
            Dim upgInstance As New AutoUpgrade()

            ' Read properties from xml file called "manifest.xml" 
            If Not IO.File.Exists(IO.Path.Combine(UpgradeDirectory, SAVED_MANIFEST)) Then
                ' manifest.xml does not exist, return an empty AutoUpgrade object
                upgInstance = New AutoUpgrade()
                Return upgInstance
            Else

                manifestFile = IO.File.OpenText(IO.Path.Combine(UpgradeDirectory, SAVED_MANIFEST))

                Try
                    ' Deserialize into object model
                    upgInstance = xmlSerializer.Deserialize(manifestFile)
                    manifestFile.Close()

                    ' If the manifest file did not contain an application base path (which it
                    ' should not, when retrieved from the server - it should contain one only
                    ' when AutoUpgrade.EXE is executed from StartUpgradeStub), set it to the
                    ' application's base directory
                    If upgInstance.ApplicationBasePath.Length = 0 Then
                        upgInstance.ApplicationBasePath = AppDomain.CurrentDomain.BaseDirectory
                    End If

                    manifestFile = Nothing
                    xmlSerializer = Nothing
                    Return upgInstance

                Catch exc As Exception
                    Throw New System.Exception("Invalid Manifest", exc)
                End Try
            End If
        End Function

        Public Shared Function Create(ByVal strManifestPath As String) As AutoUpgrade
            ' Get the manifest file from a web service.  strManifest must point to
            ' a full http path - i.e. http://localhost/AutoUpgrade.web/manifest.asmx
            '
            ' The web service must contain a method called Create() with no parameters
            '
            Dim wsClient As New devX.AutoUpgrade.WebServiceClient()

            Return wsClient.Create(strManifestPath)
        End Function

        Private Shared Function MyCertValidationCb(ByVal sender As Object, ByVal certificate As X509Certificate, _
                                            ByVal chain As X509Chain, ByVal sslPolicyErrors As System.Net.Security.SslPolicyErrors) As Boolean
            Return True
        End Function

        Private Shared Function GetWebRequest(ByVal strFilename As String) As WebRequest
            If (strFilename.Substring(0, 4) <> "ftp:") Then
                GetWebRequest = System.Net.WebRequest.Create(strFilename)
            Else
                ServicePointManager.ServerCertificateValidationCallback = AddressOf MyCertValidationCb
                Dim ftpWebRequest As FtpWebRequest = System.Net.WebRequest.Create(strFilename)
                ftpWebRequest.KeepAlive = False
                ftpWebRequest.EnableSsl = True
                ftpWebRequest.UsePassive = True
                ftpWebRequest.UseBinary = True
                ftpWebRequest.Timeout = 120000  ' increase to two minutes
                GetWebRequest = ftpWebRequest
            End If
        End Function

        Public Shared Function Create(ByVal strManifestPath As String, ByVal bThrowErrors As Boolean) As AutoUpgrade
            ' Get the manifest file from the specified Uri, create and return a new 
            ' instance of AutoUpgrade, and deserialize the object model.
            ' This version of create is for use by applications that want this class
            ' to get the manifest data from the server for them.
            Dim xmlSerializer As New XmlSerializer(GetType(AutoUpgrade), AUTOUPGRADE_XMLNS)
            Dim upgInstance As AutoUpgrade
            Dim wbrManifestfile As System.Net.WebRequest
            Dim manifestStream As System.IO.Stream

            Try
                wbrManifestfile = GetWebRequest(strManifestPath)
                manifestStream = wbrManifestfile.GetResponse.GetResponseStream

                Try
                    ' Read properties from webrequest stream (from path specified
                    ' in strManifestPath)
                    upgInstance = xmlSerializer.Deserialize(manifestStream)

                    ' Path doesn't work very well with web paths, but it *can* extract
                    ' the filename properly, which we can use to figure out the source path,
                    ' which is the same as the manifest path, with "client" appended.

                    ' We only set the SourcePath if the manifest file doesn't already 
                    ' contain one.  This way, if the host was a web service, or ASP page 
                    ' it could "redirect" clients to a different path for the 
                    ' application files by setting SourcePath in the xml manifest data

                    If upgInstance.SourcePath.Length = 0 Then
                        upgInstance.SourcePath = _
                         strManifestPath.Substring(0, Len(strManifestPath) - Len(IO.Path.GetFileName(strManifestPath)) - 1)
                        ' upgInstance.SourcePath = IO.Path.Combine(upgInstance.SourcePath, "client")
                    End If

                    ' If the manifest file did not contain an application base path (which it
                    ' should not, when retrieved from the server - it should contain one only
                    ' when AutoUpgrade.EXE is executed from StartUpgradeStub), set it to the
                    ' application's base directory
                    If upgInstance.ApplicationBasePath.Length = 0 Then
                        upgInstance.ApplicationBasePath = AppDomain.CurrentDomain.BaseDirectory
                    End If

                    manifestStream.Close()
                    manifestStream = Nothing
                    wbrManifestfile.GetResponse.Close()
                    wbrManifestfile = Nothing
                    xmlSerializer = Nothing
                    Return upgInstance

                Catch exc As Exception
                    Throw New System.Exception("Invalid Manifest", exc)
                End Try

            Catch exc As Exception
                ' If we get an exception from GetResponseStream, it means we cannot
                ' communicate with the web server (we are offline, or the manifest
                ' file does not exist).  In either case, we suppress the error and 
                ' return an object with no entries in ManifestFiles, which will return 
                ' false for IsUpgradeAvailable and allow the calling application to 
                ' continue
                If (bThrowErrors) Then
                    Throw exc
                End If
                Return New AutoUpgrade()
            End Try
        End Function


        Public Property ApplicationBasePath() As String
            ' Base directory of application (where the upgraded files get 
            ' copied to at the commit stage).  This property is generated automatically
            ' from AppDomain.CurrentDomain.BaseDirectory when the class is run from
            ' an application, then it is serialized to the xml file in StartUpgradeStub
            ' so that AutoUpgrade.EXE gets the *original* application's base directory.
            Get
                Return mstrApplicationBasePath
            End Get
            Set(ByVal Value As String)
                mstrApplicationBasePath = Value
            End Set
        End Property

        Public Property ApplicationExecutable() As String
            ' Full path to the application's executable.  Used by AutoUpgrade.EXE 
            ' to re-run the application at the end of a successful upgrade
            Get
                Return mstrApplicationExecutable
            End Get
            Set(ByVal Value As String)
                mstrApplicationExecutable = Value
            End Set
        End Property

        Public Property ApplicationName() As String
            ' Human-readable application name for captions, titles, message boxes
            Get
                Return mstrAppName
            End Get
            Set(ByVal Value As String)
                mstrAppName = Value
            End Set
        End Property


        Public Sub CommitAndRegisterFiles()
            ' Move the files from the staging directory to their normal locations,
            ' and register if required.
            Dim manCurrentAutoUpgradeFile As AutoUpgrade.File

            RaiseEvent UpgradeProgress("Committing Files and performing registration actions...", "", 0, 0, mblnCancel)

            mlngTotalBytesRead = 0
            mtimStartTime = Now

            For Each manCurrentAutoUpgradeFile In UpgradeFiles
                If manCurrentAutoUpgradeFile.Description.Trim.Length = 0 Then
                    manCurrentAutoUpgradeFile.Description = IO.Path.GetFileName(manCurrentAutoUpgradeFile.Name)
                End If
                RaiseEvent UpgradeProgress("Committing " & manCurrentAutoUpgradeFile.Description, manCurrentAutoUpgradeFile.Name, Me.PercentComplete, Me.EstimatedTimeRemaining, mblnCancel)
                If mblnCancel Then Exit For

                CommitAndRegisterSingleFile(manCurrentAutoUpgradeFile)
                mlngTotalBytesRead = mlngTotalBytesRead + manCurrentAutoUpgradeFile.Size
            Next

        End Sub

        Public Sub CommitAndRegisterSingleFile(ByVal manCurrentAutoUpgradeFile As AutoUpgrade.File)
            Dim strLocalFile As String
            Dim strCacheFile As String

            strLocalFile = IO.Path.Combine( _
             Me.ApplicationBasePath, manCurrentAutoUpgradeFile.Name)

            ' Make sure the directory/subdirectory exists
            If Not IO.Directory.Exists(IO.Path.GetDirectoryName(strLocalFile)) Then
                IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(strLocalFile))
            End If

            strCacheFile = _
             IO.Path.Combine(UpgradeDirectory, manCurrentAutoUpgradeFile.Name)

            Select Case manCurrentAutoUpgradeFile.Action
                Case AutoUpgrade.File.UpgradeAction.delete
                    ' If file exists, delete it
                    If IO.File.Exists(strLocalFile) Then
                        IO.File.Delete(strLocalFile)
                    End If
                Case AutoUpgrade.File.UpgradeAction.full
                    ' If any files require a "full upgrade", this function should 
                    ' not be called
                    Throw New ApplicationException("AutoUpgrade.CommitFiles: Upgrade manifest requires a full upgrade.")
                Case AutoUpgrade.File.UpgradeAction.copy
                    ' Copy to current location
                    IO.File.Copy(strCacheFile, strLocalFile, True)

                    If StrComp(manCurrentAutoUpgradeFile.Name, LIBDLL_FILENAME, CompareMethod.Text) <> 0 Then

                        ' Only perform registration action for files whose Action is copyFile
                        ' (and don't register AutoUpgrade.lib.dll - it must run side-by-side)
                        Call RegisterSingleFile(manCurrentAutoUpgradeFile)

                    End If

            End Select
        End Sub

        Public Sub CreateUpgradeDirectory(Optional ByVal blnDelete As Boolean = False)
            ' Create the upgrade-cache directory, used to stage the upgraded files 
            ' during the upgrade
            Dim strUpgradePath As String

            strUpgradePath = UpgradeDirectory

            If blnDelete Then
                ' If dir already exists, clear it out
                If IO.Directory.Exists(strUpgradePath) Then
                    IO.Directory.Delete(strUpgradePath, True)
                End If
            End If

            ' Create directory
            If Not IO.Directory.Exists(strUpgradePath) Then
                IO.Directory.CreateDirectory(strUpgradePath)
            End If
        End Sub

        Public Sub DownloadFiles()
            ' Download the files to be upgraded to the Upgrade cache (staging) directory
            Dim manCurrentAutoUpgradeFile As AutoUpgrade.File
            Dim datDownloadedFileTime As System.DateTime

            CreateUpgradeDirectory()
            mlngTotalBytesRead = 0

            RaiseEvent UpgradeProgress("Starting download from " & Me.SourcePath, "", 0, 0, mblnCancel)

            mtimStartTime = Now

            If IsFullUpgradeRequired() Then
                ' If any of the files signalled the need for a full upgrade, just download
                ' the upgrade installation set
                DownloadSingleFile(Me.FullUpgradeFileName, "Full upgrade")
            Else
                ' Download the files
                For Each manCurrentAutoUpgradeFile In UpgradeFiles
                    If manCurrentAutoUpgradeFile.Action = AutoUpgrade.File.UpgradeAction.copy Then
                        If StrComp(manCurrentAutoUpgradeFile.Name, LIBDLL_FILENAME, CompareMethod.Text) = 0 Then
                            ' AutoUpgrade.Lib.DLL is always downloaded, this is to avoid 
                            ' "file in use" exceptions, since it is *this* file.  If the file is
                            ' to be upgraded, include it in the manifest file as normal, and
                            ' CommitAndRegisterFiles will copy it to the application directory.
                        Else
                            DownloadSingleFile(manCurrentAutoUpgradeFile.Name, manCurrentAutoUpgradeFile.Description)
                        End If

                        ' Check that the date/time or version of the downloaded file matches the
                        ' xml file.  Warn the user if it does not.
                        If Not mblnCancel Then
                            Select Case manCurrentAutoUpgradeFile.Method
                                Case AutoUpgrade.File.CompareMethod.date
                                    ' Compare last write time.  Allow a variance of one minute in case
                                    ' the user doesn't specify milliseconds (and because on Win2k, the
                                    ' SetFileWriteTime call seems to round to the nearest minute)
#If USE_UTC Then
                                    Dim fi As IO.FileInfo = New IO.FileInfo(IO.Path.Combine(Me.UpgradeDirectory, manCurrentAutoUpgradeFile.Name))
                                    datDownloadedFileTime = fi.LastWriteTimeUtc
#Else
                                    datDownloadedFileTime = MyGetLastWriteTime(Path.Combine(UpgradeDirectory, manCurrentAutoUpgradeFile.Name))
#End If
                                    ' bs... this used to check that the file was within a minute, but that breaks
                                    ' when the US is in DST... so we don't update these files that often, so just make it +/- an hour (technically 61 minutes)
                                    ' fpos... make it 3 hours... 
                                    If System.Math.Abs(DateDiff(DateInterval.Minute, datDownloadedFileTime, CType(manCurrentAutoUpgradeFile.Version, System.DateTime))) > 185 Then
                                        RaiseEvent UpgradeProgress("The date on the downloaded file '" & manCurrentAutoUpgradeFile.Name & " - [" & datDownloadedFileTime.ToString & "]' does not match the manifest file date - [" & CType(manCurrentAutoUpgradeFile.Version, System.DateTime).ToString & "].", manCurrentAutoUpgradeFile.Name, 0, 0, mblnCancel)
                                    End If
                                Case AutoUpgrade.File.CompareMethod.version
                                    With FileVersionInfo.GetVersionInfo(Path.Combine(UpgradeDirectory, manCurrentAutoUpgradeFile.Name))
                                        If .FileVersion <> manCurrentAutoUpgradeFile.Version Then
                                            RaiseEvent UpgradeProgress("The version of the downloaded file '" & manCurrentAutoUpgradeFile.Name & "' - [" & .FileVersion & "] does not match the manifest file - [" & manCurrentAutoUpgradeFile.Version & "].", manCurrentAutoUpgradeFile.Name, 0, 0, mblnCancel)
                                        End If
                                    End With
                            End Select
                        End If
                    End If

                    If mblnCancel Then Exit For
                Next
            End If
        End Sub

        Private Shared Function IsFtp(ByVal resp As WebRequest) As Boolean
            IsFtp = TypeOf (resp) Is FtpWebRequest
        End Function

        Public Sub DownloadSingleFile(ByVal strFileName As String, Optional ByVal strDescription As String = "")
            ' Download a single file from the source to the staging directory in
            ' 16k chunks, raising UpgradeProgress events to indicate progress.
            Dim strSourceFilePath As String
            Dim strLocalPath As String
            Dim dlBuffer(BUFFERSIZE) As Byte
            Dim localFile As IO.FileStream = Nothing
            Dim lngBytesRead As Long

            strSourceFilePath = Me.SourcePath & "\" & strFileName
            Dim reqFile As System.Net.WebRequest = GetWebRequest(strSourceFilePath)
            strLocalPath = IO.Path.Combine(UpgradeDirectory, strFileName)

            Try
                ' The localpath may be a subdirectory of UpgradeDirectory, so we have
                ' to make sure it exists
                If Not IO.Directory.Exists(IO.Path.GetDirectoryName(strLocalPath)) Then
                    IO.Directory.CreateDirectory(IO.Path.GetDirectoryName(strLocalPath))
                End If

                localFile = New IO.FileStream(strLocalPath, IO.FileMode.OpenOrCreate)

                If strDescription.Trim.Length = 0 Then
                    strDescription = IO.Path.GetFileName(strFileName)
                End If
                RaiseEvent UpgradeProgress("Downloading " & strDescription, strFileName, Me.PercentComplete(), 0, mblnCancel)

                ' Download the file in 16k chunks
                With reqFile.GetResponse.GetResponseStream
                    Do
                        lngBytesRead = .Read(dlBuffer, 0, BUFFERSIZE)
                        mlngTotalBytesRead = mlngTotalBytesRead + lngBytesRead
                        localFile.Write(dlBuffer, 0, lngBytesRead)

                        RaiseEvent UpgradeProgress( _
                         "Downloading " & strDescription, _
                         strFileName, _
                         Me.PercentComplete, _
                         Me.EstimatedTimeRemaining, mblnCancel)

                    Loop Until lngBytesRead = 0 Or mblnCancel

                    .Close()
                End With

                localFile.Flush()
                localFile.Close()
                localFile = Nothing

                If Not mblnCancel Then
                    ' Set the date on the downloaded file to the date of the source
                    Dim strLstModified As String
                    If (IsFtp(reqFile)) Then
                        reqFile.GetResponse.Close()
                        reqFile = GetWebRequest(strSourceFilePath)
                        reqFile.Method = WebRequestMethods.Ftp.GetDateTimestamp
                        Using resp As FtpWebResponse = reqFile.GetResponse
                            strLstModified = resp.LastModified
                        End Using
                    Else
                        Dim resp As WebResponse = reqFile.GetResponse()
                        strLstModified = resp.Headers("Last-Modified")
                    End If
                    If Not String.IsNullOrEmpty(strLstModified) Then
                        Dim dateLastModified As Date = Date.Parse(strLstModified)

                        ' this has to be adjusted based on how far we are now from when this was created
                        Dim nowOffset As DateTimeOffset = New DateTimeOffset(DateTime.Now)
#If DEBUG Then
                        MessageBox.Show(nowOffset.ToString())
#End If
#If Amerocentric Then
                        Dim cultureInfo As CultureInfo = cultureInfo.CreateSpecificCulture("en-US")
                        Dim thenOffset As DateTimeOffset = DateTimeOffset.ParseExact(Me.OffsetFromUctWhereManifestWasOriginallyCreated, "o", cultureInfo)
#Else
                        Dim thenOffset As DateTimeOffset = DateTimeOffset.Parse(Me.OffsetFromUctWhereManifestWasOriginallyCreated)
#End If
                        dateLastModified -= nowOffset.Offset - thenOffset.Offset
#If DEBUG Then
                        Dim strMsg As String = String.Format("file: '{0}': LastModified timestamp from server: '{1}', but current offset here is '{2}' GMT and offset when created is: '{3}' GMT, so we're setting it to: '{4}'", _
                                     strSourceFilePath, strLstModified, nowOffset.Offset, thenOffset.Offset, dateLastModified)
                        System.Windows.Forms.MessageBox.Show(strMsg)
#End If
                        MySetLastWriteTime(strLocalPath, dateLastModified)
                    Else
                        Throw New ApplicationException(String.Format("Unable to get Last-Modified date on file: {0}", localFile))
                    End If
                End If

            Catch exc As Exception
                Throw New ApplicationException("Error downloading file '" & strFileName & "'", exc)

            Finally
                If Not localFile Is Nothing Then
                    localFile.Flush()
                    localFile.Close()
                End If

                reqFile.GetResponse.Close()
            End Try

        End Sub

        Public ReadOnly Property EstimatedTimeRemaining() As Long
            Get
                ' Calculate est. time remaining, based on 
                ' average time per percent * percent outstanding
                Dim lngTimeRemaining As Long
                Dim dblAverage As Double

                If Me.PercentComplete() = 0 Then
                    lngTimeRemaining = 0
                Else
                    ' Calculate average time per percent
                    dblAverage = (DateDiff(DateInterval.Second, mtimStartTime, Now) / Me.PercentComplete())
                    ' Time remaining = average per percent * percent remaining
                    lngTimeRemaining = dblAverage * (100 - Me.PercentComplete())
                    ' round to nearest 5 seconds
                    lngTimeRemaining = Int(lngTimeRemaining / 5) * 5
                End If

                Return lngTimeRemaining
            End Get
        End Property

        Public Property FullUpgradeFileName() As String
            ' The "Full upgrade" filename is the name of the setup file to be run
            ' if one or more of the manifest files require an upgrade and have
            ' an action of "full"
            Get
                Return mstrFullUpgradeName
            End Get
            Set(ByVal Value As String)
                mstrFullUpgradeName = Value
            End Set
        End Property

        Public Property OffsetFromUctWhereManifestWasOriginallyCreated() As String
            Get
                Return mtsOffsetToUCT.ToString("o")
            End Get
            Set(ByVal value As String)
                ' e.g. 8/6/2010 9:17:27 AM -05:00
#If Amerocentric Then
                ' the first time we do this (since the user's machine will have the older
                ' format), it'll fail. So be sure to back off to the older mechanism in
                ' the catch
                Try
                    Dim cultureInfo As CultureInfo = cultureInfo.CreateSpecificCulture("en-US")
                    mtsOffsetToUCT = DateTimeOffset.ParseExact(value, "o", cultureInfo)
                Catch ex As Exception
                    mtsOffsetToUCT = DateTimeOffset.Parse(value)
                End Try
#Else
                mtsOffsetToUCT = DateTimeOffset.Parse(value)
#End If
            End Set
        End Property

        ' a different implementation of GetLastWriteTime which takes differences in DST into account
        ' (problem was that the manifest was putting the wrong timestamp on the file by one hour
        ' if there was a difference between the DST flag of "now" vs. when the file was saved
        Public Shared Function MyGetLastWriteTime(ByVal strFilePath As String) As DateTime
            Dim time As DateTime = IO.File.GetLastWriteTime(strFilePath)
#If FiguringInDST Then
            Dim now As DateTime = DateTime.Now
            Dim offset As TimeSpan = New TimeSpan(1, 0, 0)
            If (now.IsDaylightSavingTime() And Not time.IsDaylightSavingTime()) Then
                time += offset
            ElseIf (Not now.IsDaylightSavingTime() And time.IsDaylightSavingTime()) Then
                time -= offset
            End If
#End If
            MyGetLastWriteTime = time
        End Function
        Shared Sub MySetLastWriteTime(ByVal strFilePath As String, ByVal time As DateTime)
#If FiguringInDST Then
            Dim now As DateTime = DateTime.Now
            Dim offset As TimeSpan = New TimeSpan(1, 0, 0)
            If (now.IsDaylightSavingTime() And Not time.IsDaylightSavingTime()) Then
                time -= offset
            ElseIf (Not now.IsDaylightSavingTime() And time.IsDaylightSavingTime()) Then
                time += offset
            End If
#End If
            IO.File.SetLastWriteTime(strFilePath, time)
        End Sub

        Public Sub GenerateManifest(ByVal strPath As String, Optional ByVal strRootPath As String = "")
            ' Generates the entire manifest, given a io.Path.  Use this function to
            ' automatically create a manifest.  This method creates entries for
            ' all files that use the copy action
            Dim strFile As String
            Dim strDirectory As String
            Dim newFileEntry As AutoUpgrade.File

            If strRootPath.Length = 0 Then
                strRootPath = strPath
            End If

            For Each strFile In IO.Directory.GetFiles(strPath)
                ' Add files to manifest, collecting info from the file system

                ' Exclude AutoUpgrade.exe.  This file is only ever downloaded if needed.
                ' AutoUpgrade.Lib.DLL is used by the client application, so it may need to be
                ' updated (so we don't exclude it).
                If Not StrComp(IO.Path.GetFileName(strFile), "AutoUpgrade.exe", CompareMethod.Text) = 0 Then
                    newFileEntry = New AutoUpgrade.File()
                    newFileEntry.Action = AutoUpgrade.File.UpgradeAction.copy

                    ' Store name and path relative to strPath
                    newFileEntry.Name = strFile.Substring(strRootPath.Length + 1)

                    ' Get other file properties
                    With FileVersionInfo.GetVersionInfo(strFile)
                        newFileEntry.Description = .FileDescription

                        newFileEntry.Optional = False
                        newFileEntry.Version = .FileVersion
                    End With

                    newFileEntry.Size = FileLen(strFile)

                    ' If version info was found, mark method as version, else date
                    If String.IsNullOrEmpty(newFileEntry.Version) Then
#If USE_UTC Then
                        Dim fi As IO.FileInfo = New IO.FileInfo(strFile)
                        newFileEntry.Version = fi.LastWriteTimeUtc.ToLongDateString & " " & fi.LastWriteTimeUtc.ToLongTimeString
#Else
                        newFileEntry.Version = MyGetLastWriteTime(strFile).ToLongDateString & " " & MyGetLastWriteTime(strFile).ToLongTimeString
#End If
                        ' only store date if no version is available
                        newFileEntry.Method = AutoUpgrade.File.CompareMethod.date
                    Else
                        newFileEntry.Method = AutoUpgrade.File.CompareMethod.version
                    End If

                    Me.ManifestFiles.Add(newFileEntry)
                End If
            Next

            ' Recurse directories
            For Each strDirectory In IO.Directory.GetDirectories(strPath)
                GenerateManifest(strDirectory, strRootPath)
            Next
        End Sub

        Public ReadOnly Property IsFullUpgradeRequired() As Boolean
            Get
                ' If any files in the upgrade are marked as requiring a "full upgrade", 
                ' then the entire upgrade is designated as requiring a full upgrade 
                ' (and this function returns true).  Else, return false.
                Dim manFile As AutoUpgrade.File

                For Each manFile In UpgradeFiles
                    If manFile.Action = AutoUpgrade.File.UpgradeAction.full Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        Public Function IsUpgradeAvailable(Optional ByVal blnStartUpgrade As Boolean = False) As Boolean
            ' Check through the entries in manifest, and add files that need 
            ' to be upgraded to the UpgradeFiles list.  Return true if one 
            ' or more files require an upgrade.
            Dim manFile As AutoUpgrade.File
            Dim blnUpgrade As Boolean

            Dim verLocalFileVersion As System.Version

            Dim datLocalFileDate As System.DateTime
            Dim datManifestFileDate As System.DateTime

            ' Clear any existing entries (that may be accidentally set in server 
            ' manifest file) 
            UpgradeFiles.Clear()

            For Each manFile In ManifestFiles
                ' For each file entry in the manifest file, use the selected "method" to
                ' check existing files.  If the file does not match, and the version or 
                ' date of the existing file is LESS than the one specified in the 
                ' manifest file, or the file could not be found, add the 
                ' manifestfile to the list of files to upgrade
                blnUpgrade = False

                Dim strFilePath As String = Path.Combine(ApplicationBasePath, manFile.Name)

                Select Case manFile.Action
                    Case AutoUpgrade.File.UpgradeAction.delete
                        ' For delete actions, schedule an upgrade if the file exists
                        If IO.File.Exists(strFilePath) Then
                            blnUpgrade = True
                        End If
                    Case AutoUpgrade.File.UpgradeAction.copy, AutoUpgrade.File.UpgradeAction.full
                        Select Case manFile.Method
                            Case AutoUpgrade.File.CompareMethod.version
                                ' Check for upgrade by file version - this method works for Win32 
                                ' applications with a version resource (pre-.NET) as well as .NET 
                                ' assemblies
                                If IO.File.Exists(strFilePath) Then
                                    verLocalFileVersion = New System.Version(System.Diagnostics.FileVersionInfo.GetVersionInfo(strFilePath).FileVersion)
                                    If verLocalFileVersion.CompareTo(New System.Version(manFile.Version)) < 0 Then
                                        blnUpgrade = True
                                    End If
                                Else
                                    blnUpgrade = True
                                End If
                            Case AutoUpgrade.File.CompareMethod.date
                                ' Use the file date to check for upgrade.  This is appropriate only 
                                ' for files that do not have a version resource (help files, 
                                ' Data files, etc)
                                If IO.File.Exists(strFilePath) Then
#If USE_UTC Then
                                    Dim fi As IO.FileInfo = New IO.FileInfo(strFilePath)
                                    datLocalFileDate = fi.LastWriteTimeUtc
#Else
                                    datLocalFileDate = MyGetLastWriteTime(strFilePath)
#End If
                                    datManifestFileDate = manFile.Version

                                    ' Allow a variance of one minute in case the user doesn't specify 
                                    ' milliseconds (and because on Win2k, the SetFileWriteTime call 
                                    ' seems to round to the nearest minute)
                                    ' bs... this used to check that the file was within a minute, but that breaks
                                    ' when the US is in DST... so we don't update these files that often, so just make it +/- an hour
                                    ' fpos... use 3 hours...
                                    If System.Math.Abs(DateDiff(DateInterval.Minute, datLocalFileDate, datManifestFileDate)) > 185 Then
#If DEBUG Then
                                        Dim strMsg As String = String.Format("4: name: '{0}', datLocalFileDate: '{1}', datManifestFileDate: '{2}'", _
                                                                             strFilePath, datLocalFileDate, datManifestFileDate)
                                        System.Windows.Forms.MessageBox.Show(strMsg)
#End If
                                        blnUpgrade = True
                                    End If
                                Else
                                    blnUpgrade = True
                                End If
                        End Select
                End Select


                If blnUpgrade Then
                    ' Add to list of files to upgrade
                    UpgradeFiles.Add(manFile)
                End If
            Next

            If UpgradeFiles.Count > 0 Then
                If blnStartUpgrade Then
                    StartUpgradeStub()
                End If

                Return True
            Else
                Return False
            End If
        End Function

        Public ReadOnly Property IsUpgradeOptional() As Boolean
            Get
                ' If any files in the upgrade are marked optional, then the entire upgrade
                ' is designated optional (and this function returns true).  Else, return 
                ' false.
                Dim manFile As AutoUpgrade.File

                For Each manFile In UpgradeFiles
                    If manFile.Optional Then
                        Return True
                    End If
                Next

                Return False
            End Get
        End Property

        Public ReadOnly Property ManifestFiles() As FileList
            ' List of files to check version or date against manifest to determine
            ' whether an upgrade is required
            Get
                Return mlstManifest
            End Get
        End Property

        Public ReadOnly Property PercentComplete() As Integer
            Get
                ' Calculate percent complete, based on total download size (sum of the
                ' file size property of all files in the UpgradeFiles list), and 
                ' the total bytes read so far.
                Return IIf(Me.TotalDownloadSize > 0, (Me.TotalBytesRead / Me.TotalDownloadSize), 0) * 100

            End Get
        End Property

        Sub RegisterSingleFile(ByVal manCurrentAutoUpgradeFile As AutoUpgrade.File)
            ' Register the specified file, using the mechanism specified in the
            ' .Registration property
            Dim strLocalFile As String
            Dim savedState As New Hashtable()

            strLocalFile = IO.Path.Combine( _
             Me.ApplicationBasePath, manCurrentAutoUpgradeFile.Name)


            ' Try running any installer classes - Suppress errors, since they will
            ' happen, if the file is not an assembly.
            ' Refer to the .NET framework help on "System.Configuration.Install Namespace"
            ' for information on implenting installer classes.
            Try
                If IO.Path.GetExtension(manCurrentAutoUpgradeFile.Name).ToLower = ".exe" OrElse _
                 IO.Path.GetExtension(manCurrentAutoUpgradeFile.Name).ToLower = ".dll" Then

                    ' CheckIfInstallable throws an exception if the assembly does not 
                    ' contain installer classes
                    System.Configuration.Install.AssemblyInstaller.CheckIfInstallable(strLocalFile)

                    With New System.Configuration.Install.AssemblyInstaller(strLocalFile, New String() {"/logfile=autoupgrade.log"})
                        .Install(savedState)
                        .Commit(savedState)

                        .Dispose()
                    End With
                End If

            Catch exc As Exception
                ' Suppress errors
            End Try
        End Sub

        Public Sub RunFullInstall()
            Dim prcInstall As New System.Diagnostics.Process()

            ' Run the full install setup program

            ' UseShellExecute=true, so msi files work OK
            prcInstall.StartInfo.UseShellExecute = True
            Process.Start(UpgradeDirectory & "\" & FullUpgradeFileName)


        End Sub

        Public Sub Save()
            ' Save the xml manifest file to the upgrade directory
            CreateUpgradeDirectory(True)
            Me.Save(IO.Path.Combine(UpgradeDirectory, SAVED_MANIFEST))
        End Sub

        Public Sub Save(ByVal strFilePath As String)
            ' Save the xml manifest file to the designated directory/filename passed in
            ' as strFilePath
            Dim xmlSerializer As XmlSerializer
            Dim xmlFile As IO.FileStream

            xmlSerializer = New XmlSerializer(GetType(AutoUpgrade), AUTOUPGRADE_XMLNS)

            ' save the manifest file in the upgrade cache directory 
            xmlFile = New IO.FileStream(strFilePath, IO.FileMode.OpenOrCreate)
            xmlSerializer.Serialize(xmlFile, Me)
            xmlFile.Close()
        End Sub

        Public Property SourcePath() As String
            ' Directory/URL to get manifest file and upgraded files from
            Get
                Return mstrSource
            End Get
            Set(ByVal Value As String)
                mstrSource = Value
            End Set
        End Property

        Public Sub StartUpgradeStub()
            ' Run the AutoUpgrade.EXE stub to upgrade the application. The application
            ' should terminate after calling this function to avoid IO/File in use 
            ' exceptions
            Dim xmlSerializer As XmlSerializer

            xmlSerializer = New XmlSerializer(GetType(AutoUpgrade), AUTOUPGRADE_XMLNS)

            ' save the manifest file in the upgrade cache directory 
            Me.ApplicationExecutable = System.Reflection.Assembly.GetEntryAssembly.Location
            Me.Save()

            ' Download a new copy of AutoUpgrade.exe/AutoUpgrade.DLL
            DownloadSingleFile(STUBEXE_FILENAME)
            DownloadSingleFile(LIBDLL_FILENAME)

            ' Run AutoUpgrade.exe
            With New System.Diagnostics.Process()
                .StartInfo.FileName = _
                 IO.Path.Combine( _
                 UpgradeDirectory, STUBEXE_FILENAME)

                .Start()
            End With

        End Sub

        Public ReadOnly Property TotalDownloadSize() As Long
            ' Calculate the total size of all files to be downloaded.  Once called,
            ' this property maintains the filesize in memory, and doesn't recalculate 
            ' it on subsequent calls
            Get
                Dim manCurrentAutoUpgradeFile As AutoUpgrade.File

                If mlngTotalSize = 0 Then
                    ' Calculate total file size 
                    For Each manCurrentAutoUpgradeFile In mlstUpgradeList
                        mlngTotalSize = mlngTotalSize + manCurrentAutoUpgradeFile.Size
                    Next
                End If
                Return mlngTotalSize
            End Get
        End Property

        Public ReadOnly Property TotalBytesRead() As Long
            ' Storage for total bytes read - used for percent complete calculation
            Get
                Return mlngTotalBytesRead
            End Get
        End Property

        Public Sub Upgrade()
            ' Upgrade the application, using the instructions in the xml Manifest.  

            If Not mblnCancel Then
                ' Download files to upgrade-cache directory
                DownloadFiles()

                ' Move files to their correct location and register 
                If IsFullUpgradeRequired() Then
                    ' Run the installer
                    RunFullInstall()
                Else
                    CommitAndRegisterFiles()
                End If

            End If
        End Sub

        Public Shared ReadOnly Property UpgradeDirectory() As String
            ' return the path of the upgrade directory.  This is either a writable folder
            ' (e.g. AppData with 'upgrade-cache' appended)...(when AutoUpgrade.Lib is 
            ' instantiated from an application), or just the base directory (when 
            ' AutoUpgrade.Lib is instantiated from a the AutoUpgrade.EXE stub)
            Get
                If StrComp(System.Reflection.Assembly.GetEntryAssembly.GetName.Name, "AutoUpgrade", CompareMethod.Text) = 0 Then
                    ' Caller is AutoUpgrade.exe (which is run from the cache dir), use BaseDirectory
                    Return AppDomain.CurrentDomain.BaseDirectory
                Else
                    ' Caller is an application, append "upgrade-cache" to BaseDirectory
                    ' Return IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "upgrade-cache")
                    Return IO.Path.Combine(System.Windows.Forms.Application.UserAppDataPath, "upgrade-cache")
                End If
            End Get
        End Property

        Public ReadOnly Property UpgradeFiles() As FileList
            ' List of files where either the version or date (depending on the compare
            ' method used) indicated an upgrade was available
            Get
                Return mlstUpgradeList
            End Get
        End Property
#End Region

#Region "SwordDownload Code"

        Public Const CstrModsDpath As String = "mods.d"
        Public Const CnFtpPort As Int32 = 21
        Public Const CstrPathSwordRemote As String = "/SWORD/"

        Public Shared Function CreateSwordDownloader() As AutoUpgrade
            Dim downloader As New AutoUpgrade()
            downloader.CreateUpgradeDirectory(True)
            Return downloader
        End Function

        Public Function AddModuleToManifest(ByVal ftp As FtpClient, ByVal strModsDfilename As String, _
                                       ByVal strModsDtempFilePath As String, ByVal strModuleDataPath As String) As Boolean
            Dim strPathSwordLocal As String = Path.Combine(UpgradeDirectory, "SWORD")
            AddModuleToManifest(ftp, strPathSwordLocal, strModsDfilename, strModsDtempFilePath, strModuleDataPath)
        End Function

        Private Function AddModuleToManifest(ftp As FtpClient, strSwordPathLocal As String, strModsName As String,
            strModsTempName As String, strRemoteDataFolder As String) As Boolean

            ' copy the downloaded (temporary) mods file to the proper path
            Dim strPathToModsFolder As String = Path.Combine(strSwordPathLocal, CstrModsDpath)
            If (Not Directory.Exists(strPathToModsFolder)) Then
                Directory.CreateDirectory(strPathToModsFolder)
            End If

            Dim strPathToMods As String = Path.Combine(strPathToModsFolder, strModsName)
            IO.File.Copy(strModsTempName, strPathToMods, True)
            Me.AddToManifest(UpgradeDirectory, strPathToMods)

            ' create the local path for the data files
            Dim strLocalPath As String = GetLocalPath(strSwordPathLocal, strRemoteDataFolder)
            If (Not Directory.Exists(strLocalPath)) Then
                Directory.CreateDirectory(strLocalPath)
            End If

            ' Get the files to it
            ftp.GetFiles(CstrPathSwordRemote + strRemoteDataFolder, strLocalPath)
            Dim strFiles As String() = Directory.GetFiles(strLocalPath)
            For Each strFile As String In strFiles
                Me.AddToManifest(UpgradeDirectory, strFile)
            Next
        End Function

        Private Shared ReadOnly _achPathDelim() As Char = {"/"}

        Private Shared Function GetLocalPath(strSwordPath As String, strModulePath As String) As String

            Dim astr As String() = strModulePath.Split(_achPathDelim, StringSplitOptions.RemoveEmptyEntries)
            For Each s As String In astr
                strSwordPath = Path.Combine(strSwordPath, s)
            Next
            Return strSwordPath
        End Function

        ' strRootPath is the path to the upgrade folder (e.g. "C:\...\"
        ' strFilepath is relative to the upgrade folder (e.g. "SWORD\mods.d\bbe.conf")
        Public Sub AddToManifest(strRootPath As String, strFile As String)
            Dim newFileEntry As File = New File()
            newFileEntry.Action = File.UpgradeAction.copy

            ' Store name and path relative to strPath
            newFileEntry.Name = strFile.Substring(strRootPath.Length + 1)

            ' Get other file properties
            With FileVersionInfo.GetVersionInfo(strFile)
                newFileEntry.Description = .FileDescription

                newFileEntry.Optional = False
                newFileEntry.Version = .FileVersion
            End With

            newFileEntry.Size = FileLen(strFile)

            ' If version info was found, mark method as version, else date
            If String.IsNullOrEmpty(newFileEntry.Version) Then
                newFileEntry.Version = MyGetLastWriteTime(strFile).ToLongDateString & " " & MyGetLastWriteTime(strFile).ToLongTimeString

                ' only store date if no version is available
                newFileEntry.Method = AutoUpgrade.File.CompareMethod.date
            Else
                newFileEntry.Method = AutoUpgrade.File.CompareMethod.version
            End If

            ManifestFiles.Add(newFileEntry)

        End Sub

        ' Run the AutoUpgrade.EXE stub (with elevated privilege) to copy the files into the protected folder.
        Public Overloads Sub StartModuleInstall()
            ' save the manifest file in the upgrade cache directory 
            Me.Save(IO.Path.Combine(UpgradeDirectory, SAVED_MANIFEST))

            ' Download a new copy of AutoUpgrade.exe/AutoUpgrade.DLL
            IO.File.Copy(Path.Combine(Me.ApplicationExecutable, STUBEXE_FILENAME), _
                         Path.Combine(UpgradeDirectory, STUBEXE_FILENAME), True)
            IO.File.Copy(Path.Combine(Me.ApplicationExecutable, LIBDLL_FILENAME), _
                         Path.Combine(UpgradeDirectory, LIBDLL_FILENAME), True)

            ' Run AutoUpgrade.exe
            With New Process()
                .StartInfo.FileName = Path.Combine(UpgradeDirectory, STUBEXE_FILENAME)
                .Start()
            End With

        End Sub

#End Region


#Region "    FileList Class    "
        Public Class FileList
            Inherits System.Collections.ArrayList

            Public Shadows Function Add(ByVal File As AutoUpgrade.File) As Integer
                Return MyBase.Add(File)
            End Function

            Default Public Shadows Property Item(ByVal index As Integer) As AutoUpgrade.File
                Get
                    Return MyBase.Item(index)
                End Get
                Set(ByVal Value As AutoUpgrade.File)
                    MyBase.Item(index) = Value
                End Set
            End Property

        End Class
#End Region

#Region "    File Class    "

        Public Class File
            Enum CompareMethod
                version
                [date]
            End Enum

            Enum UpgradeAction
                copy
                full
                delete
            End Enum

#Region "    Property storage    "

            Private mstrName As String
            ' "filename"  may include relative path information
            Private mstrDescription As String = ""
            ' human-readable description of the file
            Private mstrMethod As CompareMethod
            ' File compare method
            Private menuAction As UpgradeAction
            ' Action to perform if upgrade is required
            Private mstrVersion As String
            ' Version or last modified date (as a string)
            Private mblnOptional As Boolean
            ' Indicates whether file upgrade is optional or not
            Private mlngFileSize As Long
            ' File size for status display

#End Region

            <Xml.Serialization.XmlAttributeAttribute("name")>
            Public Property Name() As String
                ' Filename of the file.  May include path relative to the application base
                Get
                    Return mstrName
                End Get
                Set(ByVal Value As String)
                    mstrName = Value
                End Set
            End Property

            <Xml.Serialization.XmlText()>
            Public Property Description() As String
                ' Description of the file, for display to the end-user.  Optional.
                Get
                    Return mstrDescription
                End Get
                Set(ByVal Value As String)
                    mstrDescription = Value
                End Set
            End Property

            <Xml.Serialization.XmlAttributeAttribute("method")>
            Public Property Method() As CompareMethod
                ' Method to use when comparing new and existing versions.
                Get
                    Return mstrMethod
                End Get
                Set(ByVal Value As CompareMethod)
                    mstrMethod = Value
                End Set
            End Property

            <Xml.Serialization.XmlAttributeAttribute("action")>
            Public Property Action() As UpgradeAction
                ' Action to perform if an upgrade is required.
                Get
                    Return menuAction
                End Get
                Set(ByVal Value As UpgradeAction)
                    menuAction = Value
                End Set
            End Property

            <Xml.Serialization.XmlAttributeAttribute("version")>
            Public Property Version() As String
                ' File version.  Required if Method=byversion.  If the file is upgraded, '
                ' if the version does not match the file downloaded from the server, 
                ' an exception is raised

                ' if Method=bydate:  Last modified date.  If the file is upgraded, and 
                ' the date does not match the file downloaded from the server, an 
                ' exception is raised
                Get
                    Return mstrVersion
                End Get
                Set(ByVal Value As String)
                    mstrVersion = Value
                End Set
            End Property

            <Xml.Serialization.XmlAttributeAttribute("optional")>
            Public Property [Optional]() As Boolean
                ' Flag to indicate that the file can be optionally upgraded.  Not used
                ' by the automatic upgrade, but client applications can use this to 
                ' present a UI to the end user and remove entries from 
                ' AutoUpgrade.UpgradeFiles before initiating an upgrade
                Get
                    Return mblnOptional
                End Get
                Set(ByVal Value As Boolean)
                    mblnOptional = Value
                End Set
            End Property

            <Xml.Serialization.XmlAttributeAttribute("size")>
            Public Property Size() As Long
                ' Filesize used for progress meter.  If this value is not accurate, the
                ' progress meter will not be accurate.
                Get
                    Return mlngFileSize
                End Get
                Set(ByVal Value As Long)
                    mlngFileSize = Value
                End Set
            End Property
        End Class

#End Region

#Region "    Web service client    "
        < _
        System.Diagnostics.DebuggerStepThroughAttribute(), _
        System.ComponentModel.DesignerCategoryAttribute("code"), _
        System.Web.Services.WebServiceBindingAttribute(Name:="ManifestSoap", [Namespace]:="http://www.devx.com/ws/autoupgrade") _
        > _
        Friend Class WebServiceClient
            Inherits System.Web.Services.Protocols.SoapHttpClientProtocol
            < _
            System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://www.devx.com/ws/autoupgrade/Create", RequestNamespace:="http://www.devx.com/ws/autoupgrade", ResponseNamespace:="http://www.devx.com/ws/autoupgrade", Use:=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle:=System.Web.Services.Protocols.SoapParameterStyle.Wrapped) _
            > _
            Public Function Create(ByVal url As String) As devX.AutoUpgrade
                Me.Url = url
                Dim results() As Object = Me.Invoke("Create", New Object(-1) {})
                Return CType(results(0), devX.AutoUpgrade)
            End Function
        End Class
#End Region
    End Class

#End Region

End Namespace

