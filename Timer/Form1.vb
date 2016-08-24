Public Class Form1
    Dim ontimers() As Windows.Forms.Timer
    Dim offtimers() As Windows.Forms.Timer
    Dim pids() As Process
    Dim timertime() As Integer
    Dim bots As New ArrayList

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim files = My.Computer.FileSystem.GetFiles(My.Application.Info.DirectoryPath, FileIO.SearchOption.SearchAllSubDirectories, "*.exe")
        For Each file In files
            If My.Computer.FileSystem.GetFileInfo(file).Name.ToLower.Contains("bot") Then
                bots.Add(file)
            End If
        Next
        Init(bots.Count)
    End Sub
    Private Sub Init(ByVal Count As Integer)
        Dim i As Integer
        Count -= 1
        ReDim Preserve ontimers(Count)
        ReDim Preserve offtimers(Count)
        ReDim Preserve pids(Count)
        ReDim Preserve timertime(Count)
        For i = 0 To Count
            ontimers(i) = New Windows.Forms.Timer With {.Interval = 1000}
            AddHandler ontimers(i).Tick, AddressOf OnTimerTick
            offtimers(i) = New Windows.Forms.Timer With {.Interval = 1000}
            AddHandler offtimers(i).Tick, AddressOf OffTimerTick
            ListBox1.Items.Add(My.Computer.FileSystem.GetFileInfo(bots(i)).Name)
        Next i
    End Sub

    Private Sub OnTimerTick(ByVal sender As Object, ByVal e As EventArgs)
        Dim i As Integer
        i = Array.IndexOf(ontimers, sender)
        If pids(i).HasExited Then
            ontimers(i).Enabled = False
            ListBox1.Items.Item(i) = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
            Exit Sub
        End If
        timertime(i) -= 1
        If timertime(i) <= 0 Then
            ontimers(i).Enabled = False
            pids(i).Kill()
            rndtime(i, 20)
            offtimers(i).Enabled = True
        Else
            ListBox1.Items.Item(i) = "Осталось " & Int(timertime(i) / 60) & ":" & timertime(i) - (Int(timertime(i) / 60) * 60)
        End If
    End Sub

    Private Sub OffTimerTick(ByVal sender As Object, ByVal e As EventArgs)
        Dim i As Integer
        i = Array.IndexOf(offtimers, sender)
        timertime(i) -= 1
        If timertime(i) <= 0 Then
            offtimers(i).Enabled = False
            botstart(i)
            rndtime(i, 15)
            ontimers(i).Enabled = True
        Else
            ListBox1.Items.Item(i) = "Пауза " & Int(timertime(i) / 60) & ":" & timertime(i) - (Int(timertime(i) / 60) * 60)
        End If
    End Sub

    Private Sub rndtime(id As Integer, num As Integer)
        Randomize()
        Dim minutes As Integer = Int((10 * Rnd()) + 1)
        minutes += num
        Dim seconds As Integer = Int((60 * Rnd()) + 1)
        Dim time = (minutes * 60 * 1000) + (seconds * 1000)
        timertime(id) = minutes * 60 + seconds
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        For i = 0 To bots.Count - 1
            botstart(i)
            rndtime(i, 15)
            ontimers(i).Enabled = True
        Next
    End Sub
    Private Sub botstart(i As String)
        Dim dir = My.Computer.FileSystem.GetFileInfo(bots(i)).DirectoryName
        Dim file = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
        Dim prc As New ProcessStartInfo
        With prc
            .WorkingDirectory = dir
            .FileName = file
        End With
        pids(i) = New Process With {.StartInfo = prc}
        pids(i).Start()
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Dim i = ListBox1.SelectedIndex
        If ontimers(i).Enabled = True Then timertime(i) = 0 : Exit Sub
        If offtimers(i).Enabled = True Then
            offtimers(i).Enabled = False
            ListBox1.Items.Item(i) = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
            Exit Sub
        End If
        botstart(i)
        rndtime(i, 15)
        ontimers(i).Enabled = True
    End Sub
End Class
