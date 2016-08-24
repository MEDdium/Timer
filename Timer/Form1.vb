Public Class Form1
    Dim links() As LinkLabel
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
        InitLinkArray(bots.Count)
    End Sub
    Private Sub InitLinkArray(ByVal Count As Integer)
        Dim i As Integer
        Count -= 1
        ReDim Preserve links(Count)
        ReDim Preserve ontimers(Count)
        ReDim Preserve offtimers(Count)
        ReDim Preserve pids(Count)
        ReDim Preserve timertime(Count)
        For i = 0 To Count
            links(i) = New LinkLabel()
            With links(i)
                .Location = New Point(10, i * 30 + 10)
                .Text = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
            End With
            AddHandler links(i).Click, AddressOf LinkClick
            ontimers(i) = New Windows.Forms.Timer With {.Interval = 1000}
            AddHandler ontimers(i).Tick, AddressOf OnTimerTick
            offtimers(i) = New Windows.Forms.Timer With {.Interval = 1000}
            AddHandler offtimers(i).Tick, AddressOf OffTimerTick
        Next i
        Controls.AddRange(links)
    End Sub
    Private Sub LinkClick(ByVal sender As Object, ByVal e As EventArgs)
        Dim i As Integer
        i = Array.IndexOf(links, sender)
        If ontimers(i).Enabled = True Then timertime(i) = 0 : Exit Sub
        If offtimers(i).Enabled = True Then
            offtimers(i).Enabled = False
            links(i).Text = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
            Exit Sub
        End If
        botstart(i)
        rndtime(i, 15)
        ontimers(i).Enabled = True
    End Sub

    Private Sub OnTimerTick(ByVal sender As Object, ByVal e As EventArgs)
        Dim i As Integer
        i = Array.IndexOf(ontimers, sender)
        If pids(i).HasExited Then
            ontimers(i).Enabled = False
            links(i).Text = My.Computer.FileSystem.GetFileInfo(bots(i)).Name
            Exit Sub
        End If
        timertime(i) -= 1
        If timertime(i) <= 0 Then
            ontimers(i).Enabled = False
            pids(i).Kill()
            rndtime(i, 20)
            offtimers(i).Enabled = True
        Else
            links(i).Text = "Осталось " & Int(timertime(i) / 60) & ":" & timertime(i) - (Int(timertime(i) / 60) * 60)
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
            links(i).Text = "Пауза " & Int(timertime(i) / 60) & ":" & timertime(i) - (Int(timertime(i) / 60) * 60)
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
End Class
