Imports System.IO
Imports System.Reflection
Imports System.Windows.Input
Imports Microsoft.Web.WebView2.Core
Imports System.Environment

Class MainWindow
    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        webView.DefaultBackgroundColor = System.Drawing.Color.Transparent
        LoadHtml()
    End Sub
    Private Async Sub LoadHtml()
        Dim WebviewArgu = "--disable-features=msSmartScreenProtection --in-process-gpu --disable-web-security --no-sandbox --renderer-process-limit=1 --single-process"
        Dim options As New CoreWebView2EnvironmentOptions With {
            .AdditionalBrowserArguments = WebviewArgu
        }
        Dim webView2Environment = Await CoreWebView2Environment.CreateAsync(, CurrentDirectory + "\QinliliWebview2\", options)
        Await webView.EnsureCoreWebView2Async(webView2Environment)
        If Not Command() = "-debug" Then
            webView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = False
        End If
        Dim asm As Assembly = Assembly.GetExecutingAssembly()
        'MsgBox(String.Join(",", Assembly.GetExecutingAssembly().GetManifestResourceNames().ToArray()))
        Dim reader As StreamReader = New StreamReader(asm.GetManifestResourceStream("ApexWebview2.mainHtml.html"))
        Dim htmlString As String = Await reader.ReadToEndAsync()
        webView.CoreWebView2.NavigateToString(htmlString)
    End Sub

    Private Sub webView_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles webView.NavigationCompleted
        If Process.GetProcessesByName("r5apex").Length > 0 Then
            MsgBox("Game is running. Exiting game is recommended.",, "Alert")
        End If
        readCfg()
    End Sub
    Private Async Sub readCfg()
        Dim cfgPath = Environment.GetEnvironmentVariable("USERPROFILE") + "/Saved Games/Respawn/Apex/local/videoconfig.txt"
        If File.Exists(cfgPath) Then
            Dim reader = File.OpenText(cfgPath)
            Dim config As String = Await reader.ReadToEndAsync()
            reader.Close()
            config = Convert.ToBase64String(Text.Encoding.UTF8.GetBytes(config))
            Await webView.CoreWebView2.ExecuteScriptAsync("readCfg('" + config + "')")
            PBar.IsIndeterminate = False
            PBar.Value = 100
            PBar.Visibility = Visibility.Hidden
        Else
            MsgBox("No config file found. Run game first!",, "Alert")
            PBar.Value = 100
            PBar.Foreground = New SolidColorBrush(
Media.ColorConverter.ConvertFromString("#FFFF0000"))
        End If
        GC.Collect()
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
        If msg = """exit""" Then
            System.Windows.Application.Current.Shutdown()
        End If
    End Sub

    Private Sub ExitBtn_Click(sender As Object, e As RoutedEventArgs) Handles ExitBtn.Click
        System.Windows.Application.Current.Shutdown()
    End Sub

    Private Sub Title_MouseDown(sender As Object, e As MouseButtonEventArgs) Handles Title.MouseDown
        If e.ChangedButton = MouseButton.Left Then
            DragMove()
        End If
    End Sub
End Class
