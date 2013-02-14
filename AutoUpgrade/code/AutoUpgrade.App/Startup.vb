Imports System.Environment

Namespace AutoUpgrade
	Module Startup

        Sub Main()
            Debug.Fail("Debugging")
            If GetCommandLineArgs.Length > 1 Then
                Generate()
            Else
                Upgrade()
            End If
        End Sub

		Sub Generate()
			Dim upgAuto As New devX.AutoUpgrade()
            Dim strPath, strZipPath As String
            Dim strTarget As String

            If GetCommandLineArgs.Length <> 4 Then
                MsgBox( _
                 "Syntax:  To generate a manifest, use the syntax AutoUpgrade.exe " & _
                 "path target, where path is the path you want to search for files " & _
                 "to add to the manifest, and target is where you want to save the " & _
                 "manifest file.  To run an AutoUpgrade, run with no arguments.  All " & _
                 "settings are read from the manifest.xml file in the same directory " & _
                 "as AutoUpgrade.exe", MsgBoxStyle.Exclamation, "Invalid Syntax")
            Else
                ' Get the command line args
                strPath = GetCommandLineArgs(1)
                strZipPath = GetCommandLineArgs(2)
                strTarget = GetCommandLineArgs(3)

                ' If the manifest file (strTarget) already exists, delete it, just in case
                ' it is in the strPath directory - since we don't want the manifest file
                ' to contain entries for itself
                If IO.File.Exists(strTarget) Then
                    IO.File.Delete(strTarget)
                End If

                ' first clear out the zip folder and then re-create it
                devX.AutoUpgrade.ClearOutDirectory(strZipPath)
                IO.Directory.CreateDirectory(strZipPath)

                ' Auto-generate the manifest
                upgAuto.GenerateManifest(strPath, strZipPath)
                ' Save to disk
                upgAuto.Save(strTarget)
            End If

        End Sub

        Sub Upgrade()
            Dim upgAuto As devX.AutoUpgrade
            Dim strError As String

            ' Show the status form
            ' frmStatus.Show()
            ' frmStatus.Activate()
            ' Application.DoEvents()  ' get the window to repaint before going to sleep
            ' change this to loop until the OSE process isn't running anymore
            ' also, use the progress dialog to track the SWORD and upgrade downloads

            Try
                ' Open the manifest file & create a new instance of the AutoUpgrade class
                upgAuto = devX.AutoUpgrade.Create()

                Try
                    ' frmStatus.ApplicationName = upgAuto.ApplicationName

                    ' Pause for a couple of seconds to give the original app time
                    ' to shut down
                    Threading.Thread.Sleep(2000)

                    ' Start the upgrade.  Status messages are processed in the OnProgress event
                    ' AddHandler upgAuto.UpgradeProgress, AddressOf frmStatus.OnProgress
                    upgAuto.Upgrade()

                    If (MessageBox.Show("Update completed. Click 'OK' to restart OneStory Editor", "OneStory Editor Updater", MessageBoxButtons.OKCancel) = DialogResult.OK) Then
                        ' Upgrade complete.  Re-run the application unless the user clicked cancel
                        ' If upgAuto.ApplicationExecutable.Length > 0 AndAlso frmStatus.Cancel = False Then
                        If upgAuto.ApplicationExecutable.Length > 0 Then
                            With New System.Diagnostics.Process()
                                .StartInfo.FileName = upgAuto.ApplicationExecutable
                                .Start()
                            End With
                        End If
                    End If
                Catch exc As Exception
                    strError = exc.Message
                    If Not exc.InnerException Is Nothing Then
                        strError = strError & " - " & exc.InnerException.Message
                    End If
                    strError = strError & vbCrLf & vbCrLf & exc.ToString
                    MsgBox(strError, MsgBoxStyle.Critical, upgAuto.ApplicationName & " Automatic Upgrade")
                Finally
                    ' Clean up
                    ' frmStatus.Close()
                    ' frmStatus = Nothing
                    upgAuto = Nothing
                End Try

            Catch exc As Exception
                ' Error creating AutoUpgrade object
                strError = exc.Message
                If (Not exc.InnerException Is Nothing) Then
                    strError = String.Format("{1}{0}{0}{2}", Environment.NewLine, strError, exc.InnerException.Message)
                End If
                MsgBox(strError, MsgBoxStyle.Critical, "AutoUpgrade")
            End Try
        End Sub
	End Module

End Namespace