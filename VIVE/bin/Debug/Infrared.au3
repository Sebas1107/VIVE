Sleep(5000)

Dim $Flag = False

Do
   if ( WinExists("VIVE", "") = 0) Then
	  Run("VIVE.exe")
	  WinWaitActive("VIVE")

   EndIf
   Sleep(2000)
Until ($Flag = True)