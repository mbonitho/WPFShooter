Public MustInherit Class StateManager

    Public Event StateChanged(newState As StateManager)

    Protected canvas As Canvas

    Public MustOverride Sub Initialize(mainPanel As WrapPanel)

    Public MustOverride Sub MainLoop()

    Public MustOverride Sub GetInput()

    Public Sub RaiseStateChanged(newState As StateManager)
        RaiseEvent StateChanged(newState)
    End Sub


    Protected Sub initializeBackground(mainPanel As WrapPanel)
        Dim rng As New Random()

        canvas = New Canvas()
        canvas.Background = New SolidColorBrush() With {.Color = Colors.DarkBlue}
        canvas.Width = Constants.WindowWidth
        canvas.Height = Constants.WindowHeight

        Dim nbStars As Integer = 20

        For index = 1 To nbStars
            Dim star As New Rectangle()
            Dim starSize As Integer = rng.Next(2) + 1
            Dim posX As Integer = rng.Next(Constants.WindowWidth)
            Dim posY As Integer = rng.Next(Constants.WindowHeight)

            star.Fill = New SolidColorBrush() With {.Color = Colors.Yellow}
            star.Width = starSize
            star.Height = starSize

            star.Margin = New Thickness With
            {
                .Left = posX,
                .Top = posY,
                .Right = 0,
                .Bottom = 0
            }

            canvas.Children.Add(star)
        Next

        mainPanel.Children.Add(canvas)
    End Sub

End Class