Public Class MainStateManager
    Inherits StateManager

    Private Const enemySpeed As Integer = 1
    Private Const bulletSpeed As Integer = 2
    Private Const lifePointWidth As Integer = 10

    Private rng As Random

    Private lifePoints As Integer
    Private lifeBar As Rectangle

    Private ship As Spacecraft
    Private flames As Canvas

    Private previousDirection As Directions
    Private currentDirection As Directions
    Private accelerationX As Decimal = 0
    Private accelerationY As Decimal = 0

    Private shipBullets As New List(Of Bullet)
    Private shootDelay As DateTime

    Private enemyDelay As DateTime
    Private enemies As New List(Of Spacecraft)

    Public Overrides Sub GetInput()
        If Keyboard.IsKeyDown(Key.Left) Then
            If Keyboard.IsKeyDown(Key.Up) Then
                addAccel(-0.1, -0.1)
            ElseIf Keyboard.IsKeyDown(Key.Down) Then
                addAccel(-0.1, 0.1)
            Else
                addAccel(-0.2, 0)
            End If
        ElseIf Keyboard.IsKeyDown(Key.Right) Then
            If Keyboard.IsKeyDown(Key.Up) Then
                addAccel(0.1, -0.1)
            ElseIf Keyboard.IsKeyDown(Key.Down) Then
                addAccel(0.1, 0.1)
            Else
                addAccel(0.2, 0)
            End If
        ElseIf Keyboard.IsKeyDown(Key.Up) Then
            addAccel(0, -0.2)
        ElseIf Keyboard.IsKeyDown(Key.Down) Then
            addAccel(0, 0.2)
        End If

        If Keyboard.IsKeyDown(Key.Space) AndAlso DateTime.Now - shootDelay > TimeSpan.FromSeconds(0.5) Then
            initializeBullet()
            shootDelay = DateTime.Now
        End If
    End Sub

    Public Overrides Sub Initialize(mainPanel As WrapPanel)

        rng = New Random()

        lifePoints = 5

        initializeBackground(mainPanel)
        initializeLifeBar()
        initializeFlames()
        initializeShip()

        previousDirection = Directions.Up

        shootDelay = DateTime.Now
        enemyDelay = DateTime.Now
    End Sub

    Public Overrides Sub MainLoop()

        If currentDirection <> Directions.None Then
            previousDirection = currentDirection
        End If
        currentDirection = getDirection()

        setRotation(ship)

        showFlames()
        moveShip()

        checkOOB()
        decelerate()

        spawnSaucers()

        updateBullets()
        updateSaucers()
        updateLifeBar()
    End Sub


    Private Sub initializeLifeBar()
        lifeBar = New Rectangle With {.Height = 10, .Width = lifePoints * lifePointWidth}
        lifeBar.Fill = New SolidColorBrush() With {.Color = Colors.Red}
        lifeBar.Margin = New Thickness With {.Top = 10, .Left = 10}
        canvas.Children.Add(lifeBar)
    End Sub

    Private Sub initializeFlames()
        flames = New Canvas

        Dim f1 As Ellipse = New Ellipse With {.Width = 12, .Height = 24}
        f1.Fill = New SolidColorBrush With {.Color = Colors.Red}
        f1.Margin = New Thickness With
        {
            .Left = 0,
            .Top = 0,
            .Right = 0,
            .Bottom = 0
        } 'old top 20

        Dim f2 As Ellipse = New Ellipse With {.Width = 10, .Height = 24}
        f2.Fill = New SolidColorBrush With {.Color = Colors.Orange}
        f2.Margin = New Thickness With
        {
            .Left = 1,
            .Top = 0,
            .Right = 0,
            .Bottom = 0
        } 'old top 13


        flames.Children.Add(f1)
        flames.Children.Add(f2)
    End Sub

    Private Sub initializeShip()

        Dim hheight As Integer = 30
        Dim wwidth As Integer = 15

        Dim shipCanvas As New Canvas With {.Height = hheight, .Width = wwidth}
        shipCanvas.Margin = New Thickness() With
        {
            .Left = (Constants.WindowWidth - Constants.ExcedentLR) / 2,
            .Top = Constants.WindowHeight - Constants.ExcedentUD - 30,
            .Right = 0,
            .Bottom = 0
        }

        Dim body As Ellipse = New Ellipse With {.Width = 15, .Height = hheight}
        body.Fill = New SolidColorBrush With {.Color = Colors.CornflowerBlue}

        Dim reac1 As Ellipse = New Ellipse With {.Width = 10, .Height = 15}
        reac1.Fill = New SolidColorBrush With {.Color = Colors.LightBlue}
        reac1.Margin = New Thickness With
        {
            .Left = -5,
            .Top = hheight - reac1.Height,
            .Right = 0,
            .Bottom = 0
        }

        Dim glass1 As Ellipse = New Ellipse With {.Width = 12, .Height = 12}
        glass1.Fill = New SolidColorBrush With {.Color = Colors.Gray}
        glass1.Margin = New Thickness With
        {
            .Left = 1,
            .Top = 8,
            .Right = 0,
            .Bottom = 0
        }

        Dim glass2 As Ellipse = New Ellipse With {.Width = 9, .Height = 9}
        glass2.Fill = New SolidColorBrush With {.Color = Colors.Yellow}
        glass2.Margin = New Thickness With
        {
            .Left = 2.5,
            .Top = 9.5,
            .Right = 0,
            .Bottom = 0
        }

        Dim reac2 As Ellipse = New Ellipse With {.Width = 10, .Height = 15}
        reac2.Fill = New SolidColorBrush With {.Color = Colors.LightBlue}
        reac2.Margin = New Thickness With
        {
            .Left = 10,
            .Top = hheight - reac2.Height,
            .Right = 0,
            .Bottom = 0
        }

        shipCanvas.Children.Add(reac1)
        shipCanvas.Children.Add(reac2)
        shipCanvas.Children.Add(body)
        shipCanvas.Children.Add(glass1)
        shipCanvas.Children.Add(glass2)

        canvas.Children.Add(shipCanvas)

        ship = New Spacecraft(shipCanvas, Directions.None)
    End Sub

    Private Sub moveRelativeTo(x As Double, y As Double)
        ship.Margin = New Thickness With {.Left = ship.Margin.Left + x, .Top = ship.Margin.Top + y, .Right = ship.Margin.Right, .Bottom = ship.Margin.Bottom}
    End Sub

    Private Sub moveTo(x As Double, y As Double)
        ship.Margin = New Thickness With {.Left = x, .Top = y, .Right = ship.Margin.Right, .Bottom = ship.Margin.Bottom}
    End Sub


    Private Function getDirection() As Directions
        Dim dir As Directions = Directions.None

        If Keyboard.IsKeyDown(Key.Left) Then
            If Keyboard.IsKeyDown(Key.Up) Then
                dir = Directions.UpLeft
            ElseIf Keyboard.IsKeyDown(Key.Down) Then
                dir = Directions.BottomLeft
            Else
                dir = Directions.Left
            End If
        ElseIf Keyboard.IsKeyDown(Key.Right) Then
            If Keyboard.IsKeyDown(Key.Up) Then
                dir = Directions.UpRight
            ElseIf Keyboard.IsKeyDown(Key.Down) Then
                dir = Directions.BottomRight
            Else
                dir = Directions.Right
            End If
        ElseIf Keyboard.IsKeyDown(Key.Up) Then
            dir = Directions.Up
        ElseIf Keyboard.IsKeyDown(Key.Down) Then
            dir = Directions.Bottom
        End If

        Return dir
    End Function

    Private Sub addAccel(x As Double, y As Double)

        accelerationX += x
        accelerationY += y

        If accelerationX > 2 Then
            accelerationX = 2
        ElseIf accelerationX < -2 Then
            accelerationX = -2
        End If

        If accelerationY > 2 Then
            accelerationY = 2
        ElseIf accelerationY < -2 Then
            accelerationY = -2
        End If

    End Sub

    Private Sub decelerate()
        If accelerationX < 0 Then
            accelerationX += 0.05
        ElseIf accelerationX > 0 Then
            accelerationX -= 0.05
        End If

        If accelerationY < 0 Then
            accelerationY += 0.05
        ElseIf accelerationY > 0 Then
            accelerationY -= 0.05
        End If
    End Sub

    Private Sub moveShip()

        If currentDirection = Directions.BottomLeft OrElse
            currentDirection = Directions.BottomRight OrElse
            currentDirection = Directions.UpLeft OrElse
            currentDirection = Directions.UpRight Then

            moveRelativeTo(accelerationX / 2, accelerationY / 2)
        Else
            moveRelativeTo(accelerationX, accelerationY)
        End If
    End Sub

    Private Sub checkOOB()
        If ship.Margin.Left < 0 Then
            moveTo(0, ship.Margin.Top)
        ElseIf ship.Margin.Left + ship.FrameworkElement.ActualWidth + Constants.ExcedentLR > Constants.WindowWidth Then
            moveTo(Constants.WindowWidth - ship.FrameworkElement.ActualWidth - Constants.ExcedentLR, ship.Margin.Top)
        End If

        If ship.Margin.Top < 0 Then
            moveTo(ship.Margin.Left, 0)
        ElseIf ship.Margin.Top + ship.FrameworkElement.ActualHeight + Constants.ExcedentUD > Constants.WindowHeight Then
            moveTo(ship.Margin.Left, Constants.WindowHeight - ship.FrameworkElement.ActualHeight - Constants.ExcedentUD)
        End If
    End Sub

    Private Sub setRotation(elem As GraphicalElement, Optional dir As Directions = Directions.None)

        Dim ctl As UIElement = elem.FrameworkElement
        setRotation(ctl, dir)

    End Sub

    Private Sub setRotation(ctl As FrameworkElement, Optional dir As Directions = Directions.None)

        If dir = Directions.None Then
            dir = currentDirection
        End If

        Select Case dir
            Case Directions.Up
                ctl.RenderTransform = New RotateTransform With {.Angle = 0, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.UpRight
                ctl.RenderTransform = New RotateTransform With {.Angle = 45, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.Right
                ctl.RenderTransform = New RotateTransform With {.Angle = 90, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.BottomRight
                ctl.RenderTransform = New RotateTransform With {.Angle = 135, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.Bottom
                ctl.RenderTransform = New RotateTransform With {.Angle = 180, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.BottomLeft
                ctl.RenderTransform = New RotateTransform With {.Angle = 225, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.Left
                ctl.RenderTransform = New RotateTransform With {.Angle = 270, .CenterX = 0.5, .CenterY = 0.5}
            Case Directions.UpLeft
                ctl.RenderTransform = New RotateTransform With {.Angle = 315, .CenterX = 0.5, .CenterY = 0.5}
        End Select
    End Sub

    Private Sub positionFlames()
        Dim left As Integer
        Dim top As Integer

        Select Case currentDirection
            Case Directions.Up
                left = ship.Margin.Left + 0.75
                top = ship.Margin.Top + 32

            Case Directions.UpRight
                left = ship.Margin.Left - 20
                top = ship.Margin.Top + 22

            Case Directions.Right
                left = ship.Margin.Left - 34
                top = ship.Margin.Top + 0.75

            Case Directions.BottomRight

            Case Directions.Bottom
                left = ship.Margin.Left - 0.75
                top = ship.Margin.Top - 34

            Case Directions.BottomLeft

            Case Directions.Left
                left = ship.Margin.Left + 34
                top = ship.Margin.Top - 0.75

            Case Directions.UpLeft
                left = ship.Margin.Left + 25
                top = ship.Margin.Top + 22

        End Select

        flames.Margin = New Thickness With
                        {
                            .Left = left,
                            .Top = top,
                            .Right = 0,
                            .Bottom = 0
                        }

        setRotation(flames)
    End Sub

    Private Sub showFlames()

        If accelerationX <> 0 OrElse accelerationY <> 0 Then

            positionFlames()

            If Not canvas.Children.Contains(flames) Then
                canvas.Children.Insert(canvas.Children.IndexOf(ship.FrameworkElement) - 1, flames)
                flames.Visibility = Windows.Visibility.Visible
            End If

        Else
            flames.Visibility = Windows.Visibility.Hidden

            If canvas.Children.Contains(flames) Then
                canvas.Children.Remove(flames)
            End If

        End If
    End Sub


    Private Sub updateLifeBar()
        lifeBar.Width = lifePointWidth * lifePoints
    End Sub



    Private Sub initializeBullet()
        If shipBullets.Count > 3 Then
            Return
        End If

        Dim bulletDirection As Directions = currentDirection
        If bulletDirection = Directions.None Then
            If previousDirection = Directions.None Then
                bulletDirection = Directions.Up
            Else
                bulletDirection = previousDirection
            End If
        End If

        Dim r As New Rectangle With {.Width = 2, .Height = 6}
        r.Fill = New SolidColorBrush With {.Color = Colors.Violet}

        setRotation(r, bulletDirection)

        r.Margin = New Thickness With
        {
            .Left = ship.Margin.Left + ship.FrameworkElement.ActualWidth / 2,
            .Top = ship.Margin.Top + ship.FrameworkElement.ActualHeight / 2,
            .Right = 0,
            .Bottom = 0
        }

        shipBullets.Add(New Bullet(r, bulletDirection))
        canvas.Children.Add(r)
    End Sub

    Private Sub updateBullets()

        Dim bultToRemove As New List(Of Bullet)
        Dim enemiesToRemove As New List(Of Spacecraft)

        For i As Integer = 0 To shipBullets.Count - 1

            Dim bult As Bullet = shipBullets(i)

            Select Case bult.Direction

                Case Directions.UpRight
                    bult.MoveRelativeTo(1, -1)

                Case Directions.Right
                    bult.MoveRelativeTo(bulletSpeed, 0)

                Case Directions.BottomRight
                    bult.MoveRelativeTo(bulletSpeed / 2, bulletSpeed / 2)

                Case Directions.Bottom
                    bult.MoveRelativeTo(0, bulletSpeed)

                Case Directions.BottomLeft
                    bult.MoveRelativeTo(bulletSpeed / 2, -bulletSpeed / 2)

                Case Directions.Left
                    bult.MoveRelativeTo(-bulletSpeed, 0)

                Case Directions.UpLeft
                    bult.MoveRelativeTo(-bulletSpeed / 2, -bulletSpeed / 2)

                Case Directions.Up
                    bult.MoveRelativeTo(0, -bulletSpeed)

                Case Else
                    bult.MoveRelativeTo(0, -bulletSpeed)

            End Select

            'check oob
            If bult.X < 0 OrElse bult.X > Constants.WindowWidth _
                OrElse bult.Y < 0 OrElse bult.Y > Constants.WindowHeight Then
                bultToRemove.Add(bult)
            End If

            'check collision
            For Each foe As Spacecraft In enemies
                If IsCollision(foe, bult) Then
                    enemiesToRemove.Add(foe)
                    bultToRemove.Add(bult)
                End If
            Next

        Next

        For Each bult As Bullet In bultToRemove
            shipBullets.Remove(bult)
            bult.Destroy(canvas)
        Next

        For Each foe As Spacecraft In enemiesToRemove
            enemies.Remove(foe)
            foe.Destroy(canvas)
        Next

    End Sub


    Private Sub initializeSaucer(x As Integer, y As Integer)
        Dim hheight As Integer = 15
        Dim wwidth As Integer = 40

        Dim saucer As Canvas = New Canvas With {.Height = hheight, .Width = wwidth}
        saucer.Margin = New Thickness() With
        {
            .Left = x,
            .Top = y,
            .Right = 0,
            .Bottom = 0
        }

        Dim body As New Ellipse With {.Width = 30, .Height = hheight}
        body.Fill = New SolidColorBrush With {.Color = Colors.DarkRed}

        Dim body2 As New Ellipse With {.Width = 40, .Height = 20}
        body2.Fill = New SolidColorBrush With {.Color = Colors.DarkRed}
        body2.Margin = New Thickness With
        {
            .Left = -5,
            .Top = 6,
            .Right = 0,
            .Bottom = 0
        }

        Dim body3 As New Rectangle With {.Width = 30, .Height = 10}
        body3.Fill = New SolidColorBrush With {.Color = Colors.DarkRed}
        body3.Margin = New Thickness With
        {
            .Left = 0,
            .Top = 5,
            .Right = 0,
            .Bottom = 0
        }

        Dim glass1 As New Ellipse With {.Width = 5, .Height = 5}
        glass1.Fill = New SolidColorBrush With {.Color = Colors.Gray}
        glass1.Margin = New Thickness With
        {
            .Left = 2.5,
            .Top = 6,
            .Right = 0,
            .Bottom = 0
        }


        Dim glass2 As New Ellipse With {.Width = 5, .Height = 5}
        glass2.Fill = New SolidColorBrush With {.Color = Colors.Gray}
        glass2.Margin = New Thickness With
        {
            .Left = 12.5,
            .Top = 6,
            .Right = 0,
            .Bottom = 0
        }

        Dim glass3 As New Ellipse With {.Width = 5, .Height = 5}
        glass3.Fill = New SolidColorBrush With {.Color = Colors.Gray}
        glass3.Margin = New Thickness With
        {
            .Left = 22.5,
            .Top = 6,
            .Right = 0,
            .Bottom = 0
        }

        Dim light As New Ellipse With {.Width = 20, .Height = 20}
        light.Fill = New SolidColorBrush With {.Color = Colors.Yellow}
        light.Margin = New Thickness With
        {
            .Left = 5,
            .Top = 10,
            .Right = 0,
            .Bottom = 0
        }

        saucer.Children.Add(light)
        saucer.Children.Add(body2)
        saucer.Children.Add(body)
        saucer.Children.Add(body3)
        saucer.Children.Add(glass1)
        saucer.Children.Add(glass2)
        saucer.Children.Add(glass3)


        canvas.Children.Add(saucer)

        Dim s As New Spacecraft(saucer, Directions.Bottom)

        enemies.Add(s)
    End Sub

    Private Sub updateSaucers()

        Dim enemiesToRemove As New List(Of Spacecraft)

        For i As Integer = 0 To enemies.Count - 1

            Dim saucer As Spacecraft = enemies(i)

            Select Case saucer.Direction

                Case Directions.UpRight
                    saucer.MoveRelativeTo(enemySpeed / 2, -enemySpeed / 2)

                Case Directions.Right
                    saucer.MoveRelativeTo(enemySpeed, 0)

                Case Directions.BottomRight
                    saucer.MoveRelativeTo(enemySpeed / 2, enemySpeed / 2)

                Case Directions.Bottom
                    saucer.MoveRelativeTo(0, enemySpeed)

                Case Directions.BottomLeft
                    saucer.MoveRelativeTo(enemySpeed / 2, -enemySpeed / 2)

                Case Directions.Left
                    saucer.MoveRelativeTo(-enemySpeed, 0)

                Case Directions.UpLeft
                    saucer.MoveRelativeTo(-enemySpeed / 2, -enemySpeed / 2)

                Case Directions.Up
                Case Else
                    saucer.MoveRelativeTo(0, -enemySpeed)

            End Select

            'check oob
            If saucer.X < 0 OrElse saucer.X > Constants.WindowWidth _
                OrElse saucer.Y < 0 OrElse saucer.Y > Constants.WindowHeight Then
                enemiesToRemove.Add(saucer)
            End If

            'check collision with player
            For Each foe As Spacecraft In enemies
                If IsCollision(foe, ship) Then
                    enemiesToRemove.Add(foe)
                    enemiesToRemove.Add(saucer)

                    lifePoints -= 1
                    If lifePoints = 0 Then
                        RaiseStateChanged(New TitleStateManager())
                    End If
                End If
            Next

        Next

        For Each foe As Spacecraft In enemiesToRemove
            enemies.Remove(foe)
            foe.Destroy(canvas)
        Next

    End Sub


    Private Function IsCollision(one As GraphicalElement, two As GraphicalElement) As Boolean
        Dim notColliding As Boolean = (one.X < two.X AndAlso one.X + one.Width < two.X) OrElse
                                      (one.X > two.X + two.Width AndAlso one.X + one.Width > two.X) OrElse
                                      (one.Y < two.Y AndAlso one.Y + one.Height < two.Y) OrElse
                                      (one.Y > two.Y + two.Height AndAlso one.Y + one.Height > two.Y)
        Return Not notColliding
    End Function

    Private Sub spawnSaucers()
        Dim x As Integer = rng.Next(Constants.WindowWidth)
        Dim y As Integer = rng.Next(50)

        If DateTime.Now - enemyDelay > TimeSpan.FromSeconds(5) Then
            initializeSaucer(x, y)
            enemyDelay = DateTime.Now()
        End If

    End Sub

End Class
