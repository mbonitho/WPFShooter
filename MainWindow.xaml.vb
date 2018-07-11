Imports System.Windows.Threading
Imports System.Threading

Class MainWindow

    Private timer As DispatcherTimer

    Private WithEvents currentState As StateManager

    Public Sub New()

        ' Cet appel est requis par le concepteur.
        InitializeComponent()
        Me.Width = Constants.WindowWidth
        Me.Height = Constants.WindowHeight

        currentState = New TitleStateManager()

        initialize()
        startMainLoop()
    End Sub

    Private Sub initialize()
        mainPanel.Children.Clear()
        currentState.Initialize(mainPanel)
    End Sub

    Private Sub startMainLoop()
        timer = New DispatcherTimer With {.Interval = TimeSpan.FromMilliseconds(2.5)}
        AddHandler timer.Tick, AddressOf MainLoop
        timer.Start()
    End Sub

    Private Sub getInput()
        currentState.GetInput()
    End Sub

    Private Sub MainLoop(sender As Object, e As EventArgs)
        getInput()
        currentState.MainLoop()
    End Sub

    Private Sub StateManagerChanged(newState As StateManager) Handles currentState.StateChanged
        currentState = newState
        initialize()
    End Sub

End Class
