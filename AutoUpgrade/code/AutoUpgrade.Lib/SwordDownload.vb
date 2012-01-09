Imports System.Xml.Serialization
Imports devX
Imports System.Text.RegularExpressions
Imports System.IO
Imports Starksoft.Net.Ftp

Namespace devX

    Public Class SwordDownload
        Inherits AutoUpgrade

        Public Const CstrModsDpath As String = "mods.d"
        Public Const CnFtpPort As Int32 = 21
        Public Const CstrPathSwordRemote As String = "/SWORD/"

        Public Sub New()

        End Sub

        Public Function DownloadModule(ByVal ftp As FtpClient, ByVal strModsDfilename As String, _
                                       ByVal strModsDtempFilePath As String, ByVal strModuleDataPath As String) As Boolean
            Dim strPathSwordLocal As String = Path.Combine(ApplicationBasePath, "SWORD")
            DownloadModule(ftp, strPathSwordLocal, strModsDfilename, strModsDtempFilePath, strModuleDataPath)
        End Function

        Public Shared Function Create(ByVal ftp As FtpClient, ByVal strMod As String) As SwordDownload
            ' Get all the ini files from the specified path + 'mods.d'
            ' create a new ftpclient object with the host and port number to use
            ' set the security protocol to use - in this case we are instructing the FtpClient to use either
            ' the SSL 3.0 protocol or the TLS 1.0 protocol depending on what the FTP server supports
            Dim ftp As FtpClient = New FtpClient(strHost, CnFtpPort, FtpSecurityProtocol.Tls1OrSsl3Explicit)

            ' register an event hook so that we can view and accept the security certificate that is given by the FTP server
            AddHandler ftp.ValidateServerCertificate, AddressOf FtpValidateServerCertificate

            Try
                ftp.Open(strUsername, strPassword)
                Dim files As FtpItemCollection = ftp.GetDirList(CstrPathSwordRemote + CstrModsDpath)
                For Each file As FtpItem In files
                    Dim strTempFilename = Path.GetTempFileName()
                    ftp.GetFile(file.FullPath, strTempFilename, FileAction.Create)
                    Dim strShortCode, strDescription, strDataPath As String
                    If (Not GetInformation(strTempFilename, strShortCode, strDescription, strDataPath)) Then
                        Continue For
                    End If
                    Console.WriteLine("{0}: {1}, DataPath: {2}", strShortCode, strDescription, strDataPath)

                    DownloadModule(ftp, CstrPathSwordLocal, file.Name, strTempFilename, strDataPath)
                Next
            Catch exc As FtpAuthenticationException
                Console.WriteLine("Certificate authentication was rejected or is invalid.")
            Catch exc As Exception
                Console.WriteLine(exc.Message)
            Finally
                ftp.Close()
            End Try

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

        Public Function DownloadModule(ftp As FtpClient, strSwordPathLocal As String, strModsName As String,
            strModsTempName As String, strRemoteDataFolder As String) As Boolean

            ' copy the downloaded (temporary) mods file to the proper path
            Dim strPathToModsFolder As String = Path.Combine(strSwordPathLocal, CstrModsDpath)
            If (!Directory.Exists(strPathToModsFolder)) Then
                Directory.CreateDirectory(strPathToModsFolder)
            End If

            Dim strPathToMods As String = Path.Combine(strPathToModsFolder, strModsName)
            IO.File.Copy(strModsTempName, strPathToMods, True)

            ' create the local path for the data files
            Dim strLocalPath As String = GetLocalPath(strSwordPathLocal, strRemoteDataFolder)

            ' Get the files to it
            ftp.GetFiles(CstrPathSwordRemote + strRemoteDataFolder, strLocalPath)
        End Function

        Private Shared ReadOnly _achPathDelim() As Char = {"/"}

        Private Shared Function GetLocalPath(strSwordPath As String, strModulePath As String) As String

            Dim astr As String() = strModulePath.Split(_achPathDelim, StringSplitOptions.RemoveEmptyEntries)
            For Each s As String In astr
                strSwordPath = Path.Combine(strSwordPath, s)
            Next
            Return strSwordPath
        End Function

        Shared ReadOnly RegexModsReaderShortCode As Regex = New Regex("\[(.+?)\]", RegexOptions.Compiled Or RegexOptions.Singleline)
        Shared ReadOnly RegexModsReaderDesc As Regex = New Regex("Description=(.+?)[\n\r]", RegexOptions.Compiled Or RegexOptions.Singleline)
        Shared ReadOnly RegexModsReaderDataPath As Regex = New Regex("DataPath=\.\/(.+?)[\n\r]", RegexOptions.Compiled Or RegexOptions.Singleline)

        Private Shared Function GetInformation(strFilename As String, ByRef strShortCode As String, ByRef strDescription As String, _
            ByRef strDataPath As String) As Boolean

            strShortCode = strDescription = strDataPath = Nothing
            Dim strContents As String = IO.File.ReadAllText(strFilename)

            Dim match As Match = RegexModsReaderShortCode.Match(strContents)
            If (match.Groups.Count <> 2) Then
                Return False
            End If

            strShortCode = match.Groups(1).Value

            match = RegexModsReaderDesc.Match(strContents)
            If (match.Groups.Count <> 2) Then
                Return False
            End If

            strDescription = match.Groups(1).Value

            match = RegexModsReaderDataPath.Match(strContents)
            If (match.Groups.Count <> 2) Then
                Return False
            End If

            strDataPath = match.Groups(1).Value

            If (strDataPath(strDataPath.Length - 1) <> "/") Then
                strDataPath = strDataPath + '/'
            End If
            Return True
        End Function

        Private Shared Sub FtpValidateServerCertificate(sender As Object, e As ValidateServerCertificateEventArgs)
            e.IsCertificateValid = True
        End Sub

    End Class
End Namespace