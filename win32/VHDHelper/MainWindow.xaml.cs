using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace VHDHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int OpenVhdImageFileError = -1;
        private const int InvaildOrBrokenVhdImageFile = -2;
        private const int NonsuportVhdIamgeFile = -3;
        private const int InvaildDataFile = -4;
        private const int LbaOutOfRange = -5;
        private const int OpenDataFileError = -6;

        public MainWindow()
        {
            InitializeComponent();
        }

        #region 响应事件

        private void SelectVHDFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "VHD File (*.vhd)|*.vhd"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.VHDTextBox.Text = dialog.FileName;
            }
        }

        private void SelectDataFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Data File (*.*)|*.*"
            };
            var result = dialog.ShowDialog();
            if (result == true)
            {
                this.DataTextBox.Text = dialog.FileName;
            }
        }

        private void SectorTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex(@"[^0-9]");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void WriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.VHDTextBox.Text.Length == 0)
            {
                MessageBox.Show("VHD 文件路径不能为空");
                return;
            }

            if (this.DataTextBox.Text.Length == 0)
            {
                MessageBox.Show("Data 文件路径不能为空");
                return;
            }

            if (this.SectorTextBox.Text.Length == 0)
            {
                MessageBox.Show("起始扇区不能为空");
                return;
            }

            string vhdName = this.VHDTextBox.Text;
            string dataName = this.DataTextBox.Text;
            int lba = 0;
            try
            {
                lba = Convert.ToInt32(this.SectorTextBox.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            Enable(false);
            DateTime startTime = DateTime.Now;
            LogLine(startTime.ToString());
            LogLine("向 " + vhdName + " 扇区 " + lba);
            LogLine("写入 " + dataName);
            int result = 0;
            this.StatusLabel.Content = "执行中";
            var worker = new BackgroundWorker();
            worker.DoWork += (o, e) => {
                result = VHDCaller.Write(vhdName, lba, dataName);
            };
            worker.RunWorkerCompleted += (o, e) => {
                this.StatusLabel.Content = "完成";
                switch (result)
                {
                    case OpenVhdImageFileError:
                        LogLine("打开 VHD 镜像文件失败");
                        break;
                    case InvaildOrBrokenVhdImageFile:
                        LogLine("VHD 镜像文件损坏");
                        break;
                    case NonsuportVhdIamgeFile:
                        LogLine("不支持的 VHD 镜像文件");
                        break;
                    case InvaildDataFile:
                        LogLine("非法的数据文件");
                        break;
                    case LbaOutOfRange:
                        LogLine("起始扇区超出范围");
                        break;
                    case OpenDataFileError:
                        LogLine("打开数据文件失败");
                        break;
                    default:
                        LogLine("写入 " + result + " byte(s)，耗时 " + (DateTime.Now - startTime).TotalSeconds + "s");        
                        break;
                }
                LogLine("完成\r\n");
                Enable(true);
                this.Activate();
            };
            worker.RunWorkerAsync();
        }

        #endregion

        private void Enable(bool isEnable)
        {
            this.SelectVHDFileButton.IsEnabled = isEnable;
            this.SelectDataFileButton.IsEnabled = isEnable;
            this.SectorTextBox.IsReadOnly = !isEnable;
            this.WriteButton.IsEnabled = isEnable;
        }

        private void LogLine(string msg)
        {
            this.LogTextBox.AppendText(msg + "\r\n");
            this.LogTextBox.ScrollToEnd();
        }
    }
}
