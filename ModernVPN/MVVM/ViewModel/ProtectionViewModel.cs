using ModernVPN.Core;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ModernVPN.MVVM.ViewModel
{
    internal class ProtectionViewModel : ObservableObject
    {
        public ObservableCollection<ServerModel> Servers { get; set; }

        public GlobalViewModel Global { get; } = GlobalViewModel.Instance;

        private string _connectionStatus;
        private string _connected = "Connect";

        public string ConnectionStatus
        {
            get { return _connectionStatus; }
            set
            {
                _connectionStatus = value;
                OnPropertyChanged();
            }
        }

        public string Connected
        {
            get { return _connected; }
            set
            {
                _connected = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand ConnectCommand { get; set; }

        public ProtectionViewModel()
        {
            Servers = new ObservableCollection<ServerModel>();
            for (int i = 0; i < 50; i++)
            {
                Servers.Add(new ServerModel
                {
                    Country = "USA"
                });
            }

            ConnectCommand = new RelayCommand(o =>
            {
                if (Connected == "Connect")
                {
                    Task.Run(() =>
                    {
                        ConnectionStatus = "Connecting...";
                        var process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        process.StartInfo.ArgumentList.Add(@"/c rasdial MyServer vpnbook 3ev7r8m /phonebook:./VPN/VPN.pbk");
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.WaitForExit();

                        switch (process.ExitCode)
                        {
                            case 0:
                                Debug.WriteLine("Success!");
                                ConnectionStatus = "Connected!";
                                Connected = "Disconnect";
                                break;

                            case 691:
                                Debug.WriteLine("Wrong credentials!");
                                ConnectionStatus = "Try a different password?";
                                break;

                            default:
                                Debug.WriteLine($"Error: {process.ExitCode}");
                                break;
                        }
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        ConnectionStatus = "Disconnecting...";
                        var process = new Process();
                        process.StartInfo.FileName = "cmd.exe";
                        process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
                        process.StartInfo.ArgumentList.Add(@"/c rasdial /d");
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;

                        process.Start();
                        process.WaitForExit();

                        switch (process.ExitCode)
                        {
                            case 0:
                                Debug.WriteLine("Success!");
                                ConnectionStatus = "Disconnected!";
                                Connected = "Connect";
                                break;

                            default:
                                Debug.WriteLine($"Error: {process.ExitCode}");
                                break;
                        }
                    });
                }
                
            });
        }

        private void ServerBuilder()
        {
            var address = "us1.vpnbook.com";
            var FolderPath = $"{Directory.GetCurrentDirectory()}/VPN";
            var PbkPath = $"{FolderPath}/{address}.pbk";

            if (!Directory.Exists(FolderPath))
                Directory.CreateDirectory(FolderPath);

            if (File.Exists(PbkPath))
            {
                MessageBox.Show("Connection already exists!");
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("[MyServer]");
            sb.AppendLine("MEDIA=rastapi");
            sb.AppendLine("Port=VPN2-0");
            sb.AppendLine("Device=WAN Miniport (IKEv2)");
            sb.AppendLine("DEVICE=vpn");
            sb.AppendLine($"Phonenumber={address}");
            File.WriteAllText(PbkPath, sb.ToString());
        }
    }
}