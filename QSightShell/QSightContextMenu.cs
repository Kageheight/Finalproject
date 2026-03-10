using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

[ComVisible(true)]
[COMServerAssociation(AssociationType.AllFiles)]
public class QSightContextMenu : SharpContextMenu
{
    protected override bool CanShowMenu()
    {
        return true;
    }

    protected override ContextMenuStrip CreateMenu()
    {
        var menu = new ContextMenuStrip();
        var item = new ToolStripMenuItem("Q-Sight 분석");

        item.Click += (sender, args) =>
        {
            SendScanCommand();
        };

        menu.Items.Add(item);

        return menu;
    }

    private void SendScanCommand()
    {
        try
        {
            using var pipe = new NamedPipeClientStream(".", "QSightPipe", PipeDirection.Out);
            pipe.Connect(500);

            using var writer = new StreamWriter(pipe, Encoding.UTF8);
            var msg = new { Command = "SCAN", Path = SelectedItemPaths.First() };
            writer.WriteLine(JsonSerializer.Serialize(msg));
            writer.Flush();
        }
        catch (Exception ex)
        {
            MessageBox.Show("QSight 메인 앱이 실행 중인지 확인해주세요.");
        }
    }
}