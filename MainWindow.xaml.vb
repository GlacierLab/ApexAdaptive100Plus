﻿Imports System.IO
Imports System.Reflection
Imports Microsoft.Web.WebView2.Core

Class MainWindow
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        LoadHtml()
    End Sub
    Private Async Sub LoadHtml()
        Await webView.EnsureCoreWebView2Async()
        If Not Command() = "-debug" Then
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = False
        End If
        PBar.IsIndeterminate = False
        PBar.Value = 25
        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        'MsgBox(String.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames().ToArray()))
        Dim reader As StreamReader = New StreamReader(asm.GetManifestResourceStream("ApexWebview2.mainHtml.html"))
        Dim htmlString As String = Await reader.ReadToEndAsync()
        PBar.Value = 50
        webView.CoreWebView2.NavigateToString(htmlString)
    End Sub

    Private Sub webView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles webView.NavigationCompleted
        PBar.Value = 80
        If Process.GetProcessesByName("r5apex").Length > 0 Then
            MsgBox("Game is running. Exiting game is recommended.",, "Alert")
        End If
        readCfg()
    End Sub
    Private Async Sub readCfg()
        Dim config As String = File.ReadAllText(Environment.GetEnvironmentVariable("USERPROFILE") + "/Saved Games/Respawn/Apex/local/videoconfig.txt")
        config = Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(config))
        Await webView.CoreWebView2.ExecuteScriptAsync("readCfg('" + config + "')")
        PBar.Value = 100
        PBar.Visibility = Visibility.Hidden
    End Sub
    Dim waitForconfig As Boolean
    Private Sub webView_WebMessageReceived(sender As Object, e As CoreWebView2WebMessageReceivedEventArgs) Handles webView.WebMessageReceived
        Dim msg As String = e.WebMessageAsJson
        If msg = """readCfg""" Then
            readCfg()
        End If
        If waitForconfig = True Then
            waitForconfig = False
            Dim config As String = Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(msg.Replace("""", "")))
            File.WriteAllText(Environment.GetEnvironmentVariable("USERPROFILE") + "/Saved Games/Respawn/Apex/local/videoconfig.txt", config)
        End If
        If msg = """writeCfg""" Then
            waitForconfig = True
        End If
    End Sub
End Class
