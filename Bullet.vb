Public Class Bullet
    Inherits GraphicalElement

    Public Sub New(rect As Rectangle, dir As Directions)
        _frameworkElement = rect
        Direction = dir
    End Sub

End Class