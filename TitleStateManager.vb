Public Class TitleStateManager
    Inherits StateManager

    Private lblTitle As Label

    Private blinkDelay As DateTime

    Public Overrides Sub GetInput()
        If Keyboard.IsKeyDown(Key.Enter) OrElse Keyboard.IsKeyDown(Key.Space) Then
            RaiseStateChanged(New MainStateManager())
        End If
    End Sub

    Public Overrides Sub Initialize(mainPanel As WrapPanel)
        initializeBackground(mainPanel)

        Dim lblInfo As New Label()
        lblInfo.Foreground = New SolidColorBrush With {.Color = Colors.White}
        lblInfo.Content = "Hit enter or space"
        lblInfo.Margin = New Thickness() With
                        {
                            .Top = Constants.WindowHeight - 50,
                            .Left = 20
                        }


        lblTitle = New Label()
        lblTitle.Foreground = New SolidColorBrush With {.Color = Colors.White}
        lblTitle.Content = "SPACE SHOOTER "
        lblTitle.FontSize = 14
        lblTitle.Margin = New Thickness() With
                        {
                            .Top = Constants.WindowHeight - 200,
                            .Left = 15
                        }


        Canvas.Children.Add(lblInfo)
        Canvas.Children.Add(lblTitle)

        blinkDelay = DateTime.Now()
    End Sub

    Public Overrides Sub MainLoop()
        If DateTime.Now - blinkDelay > TimeSpan.FromSeconds(1) Then
            If lblTitle.Visibility = Visibility.Visible Then
                lblTitle.Visibility = Visibility.Hidden
            Else
                lblTitle.Visibility = Visibility.Visible
            End If
            blinkDelay = DateTime.Now()
        End If
    End Sub
End Class
