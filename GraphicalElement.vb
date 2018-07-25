Public MustInherit Class GraphicalElement

    Protected _frameworkElement As FrameworkElement
    Public Property Direction As Directions

    Public Property Margin As Thickness
        Get
            Return _frameworkElement.Margin
        End Get
        Set(value As Thickness)
            _frameworkElement.Margin = value
        End Set
    End Property

    Public ReadOnly Property FrameworkElement As FrameworkElement
        Get
            Return _frameworkElement
        End Get
    End Property

    Public ReadOnly Property X As Integer
        Get
            Return _frameworkElement.Margin.Left
        End Get
    End Property

    Public ReadOnly Property Y As Integer
        Get
            Return _frameworkElement.Margin.Top
        End Get
    End Property

    Public ReadOnly Property Height As Double
        Get
            Return _frameworkElement.ActualHeight
        End Get
    End Property

    Public ReadOnly Property Width As Double
        Get
            Return _frameworkElement.ActualWidth
        End Get
    End Property

    Private Sub moveTo(x As Double, y As Double)
        Margin = New Thickness With {.Left = x, .Top = y, .Right = Margin.Right, .Bottom = Margin.Bottom}
    End Sub

    Public Sub MoveRelativeTo(x As Double, y As Double)
        _frameworkElement.Margin = New Thickness With {.Left = _frameworkElement.Margin.Left + x, .Top = _frameworkElement.Margin.Top + y, .Right = _frameworkElement.Margin.Right, .Bottom = _frameworkElement.Margin.Bottom}
    End Sub

    Sub Destroy(canvas As Canvas)
        canvas.Children.Remove(_frameworkElement)
        _frameworkElement = Nothing
    End Sub

End Class