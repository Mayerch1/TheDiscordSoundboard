'Big thanks to Philipp Sumi on hardcodet.net (http://www.hardcodet.net/author/philipp)
'and to Robert (https://robertbigec.wordpress.com/)
'(no responsibility for the content of those links)

Option Explicit

Const msiOpenDatabaseModeTransact = 1
Dim argNum, argCount:argCount = Wscript.Arguments.Count

Dim openMode : openMode =  msiOpenDatabaseModeTransact

'This script adds a type 51 custom action into a given MSI database that
'ensures the ARPINSTALLLOCATION property is set to the primary install location
'during installation. This causes the InstallLocation
'registry key to be set in the registry.


' Connect to Windows installer object
On Error Resume Next
Dim installer : Set installer = Nothing
Set installer = Wscript.CreateObject("WindowsInstaller.Installer") :
CheckError

' Open database
Dim databasePath:databasePath = Wscript.Arguments(0)
Dim database : Set database = installer.OpenDatabase(databasePath, openMode) : CheckError

' Process SQL statements
Dim query, view, record, message, rowData, columnCount, delim, column

query = "INSERT INTO `CustomAction` (`Action`, `Type`, `Source`, `Target`) VALUES ('SetARPINSTALLLOCATION', '51', 'ARPINSTALLLOCATION', '[TARGETDIR]')"  
Set view = database.OpenView(query) : CheckError
view.Execute : CheckError

query = "INSERT INTO `InstallExecuteSequence` (`Action`, `Condition`, `Sequence`) VALUES ('SetARPINSTALLLOCATION', 'NOT Installed', '1401')"
Set view = database.OpenView(query) : CheckError
view.Execute : CheckError

database.Commit

If Not IsEmpty(message) Then Wscript.Echo message
Wscript.Quit 0

Sub CheckError
  Dim message, errRec
  If Err = 0 Then Exit Sub
  message = Err.Source & " " & Hex(Err) & ": " & Err.Description
  If Not installer Is Nothing Then
    Set errRec = installer.LastErrorRecord
    If Not errRec Is Nothing Then message = message & vbLf & errRec.FormatText
  End If
Fail message
End Sub

Sub Fail(message)
  Wscript.Echo message
  Wscript.Quit 2
End Sub 