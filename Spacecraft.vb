Public Class Spacecraft
    Inherits GraphicalElement

    Public Sub New(canvas As Canvas, dir As Directions)
        _frameworkElement = canvas
        Direction = dir
    End Sub

End Class