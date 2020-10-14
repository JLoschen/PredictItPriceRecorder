'-- Copyright © 2010 Menard, Inc.  Eau Claire, WI. All rights reserved.

Imports System.AddIn.Hosting
Imports System.Threading.Tasks
Imports BoaDataEngine.Library
Imports Common.Logging
Imports Common.Logging.Log4Net
Imports Menards.Merch.BOA.Core
Imports Menards.Merch.DataEngine.API
Imports Menards.Merch.DataEngine.API.Plugins
Imports Menards.Merch.DataEngine.HostView

Public Class svcSkuDataEngine

    Private _log As ILog
    Private Shared ReadOnly PluginDirectory As String = AppDomain.CurrentDomain.SetupInformation.ApplicationBase & "Plugins"
    Private _pluginList As List(Of DataEngineRunner)
    Private _taskList As List(Of Task(Of Boolean))

    ''' <summary>
    ''' Creates a new instance of the Sku Data Engine service class.
    ''' </summary>
    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()
    End Sub

    Protected Overrides Sub OnStart(args() As String)
        Try
            [Global].HostServerType = GetHostServerType(args)
            [Global].TeradataHostServerType = GetTeraDataHostServerType(args)
            Dim testEmail = GetTestEmail(args)

            _log = LogManager.Adapter.GetLogger(GetType(Log4NetLoggerFactoryAdapter))

            _log.Info(Function(m) m("The Boa Data Engine service was started"))

            ' Needs to be run in a Thread because a Service will not start if it takes longer than 
            ' 30 seconds to exit the OnStart method...
            Task.Run(Sub()
                         Dim pluginManager as new PluginManager(PluginDirectory)
                         pluginManager.RebuildAddInStore()
                         _pluginList = pluginManager.ValidPlugins(GetType(IDataEngineHostView)) _
                                                    .Select(Function(t) New DataEngineRunner(New Plugin(t) With {.TestEmail = testEmail, 
                                                                                                                 .HostServerType = [Global].HostServerType,
                                                                                                                 .TeradataHostServerType = [Global].TeradataHostServerType})).ToList() _

                         _taskList = _pluginList.Select(Function(p)
                                                            _log.Info(Function(m) m($"{p.Plugin.Name} is starting with /host={p.Plugin.HostServerType} and /tdhost={p.Plugin.TeradataHostServerType}"))
                                                            return p.Run().ContinueWith(Function(r)
                                                                                            If r.Exception IsNot Nothing Then
                                                                                                _log.Info(Function(m) m($"Starting {p.Plugin.Name} ended in error:"))
                                                                                                _log.Info(Function(m) m(r.Exception.ToString()))
                                                                                                Return False
                                                                                            Else If Not r.Result
                                                                                                _log.Info(Function(m) m($"Starting {p.Plugin.Name} ended in error"))
                                                                                            Else
                                                                                                _log.Info(Function(m) m($"{p.Plugin.Name} has been stopped without"))
                                                                                            End If
                                                                                            
                                                                                            Return r.Result
                                                                                        End Function)
                                                        End Function).ToList()

                         _log.Info($"The service was initialized on host={[Global].HostServerType} and tdhost={[Global].TeradataHostServerType}")
                         
                         Task.WaitAll(_taskList.ToArray())
                     End Sub).ContinueWith(Sub(t)
                                                If t.Exception IsNot Nothing Then
                                                    _log.Fatal(Function(m) m("Unhandled thread exception encountered", t.Exception))
                                                   _log.Fatal(t.Exception.ToString())
                                                    [Stop]()
                                                End If
                                           End Sub)
        Catch ex As Exception
            _log.Error(Function(m) m("Exception in OnStart method."), ex)
            HelperMethods.SendEmailWithLog("The following exeception occurred in the OnStart method:", ex, True, [Global].HostServerType.ToString())
            Me.Stop()
        End Try
    End Sub

    Protected Overrides Sub OnStop()
        Try
            _pluginList.ForEach(Sub(p) p.Stop())
            Task.WaitAll(_taskList.ToArray())

            _log.Info(Function(m) m("The Boa Data Engine service was stopped"))
        Catch ex As Exception
            _log.Error(Function(m) m(""), ex)
            HelperMethods.SendEmailWithLog("The following exeception occurred in the OnStop method.", ex, False, [Global].HostServerType.ToString())
        End Try
    End Sub

    Private Shared Function GetHostServerType(args() As String) As HostServerType
        Dim argName = "/host="
        Dim argValue = args.FirstOrDefault(Function(a) a.ToLower().StartsWith(argName))?.Remove(0, argName.Length).ToLower()

        Dim result As HostServerType
        Return If([Enum].TryParse(argValue, True, result), result, HostServerType.Live)
    End Function

    Private Shared Function GetTeraDataHostServerType(args() As String) As HostServerType
        Dim argName = "/tdhost="
        Dim argValue = args.FirstOrDefault(Function(a) a.ToLower().StartsWith(argName))?.Remove(0, argName.Length).ToLower()
        If(argValue Is Nothing)
            Return GetHostServerType(args) ' Default to host server type if tera data was not explicitly specified
        End If

        Dim result As HostServerType
        Return If([Enum].TryParse(argValue, True, result), result, HostServerType.Live)
    End Function

    Private Shared Function GetTestEmail(args() As String) As string
        Dim argName = "/testemail="
        Dim argValue = args.FirstOrDefault(Function(a) a.ToLower().StartsWith(argName))?.Remove(0, argName.Length).ToLower()

        return If(String.IsNullOrWhiteSpace(argValue), Nothing,  argValue)
    End Function

End Class