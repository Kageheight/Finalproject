using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO.Pipes;
using System.IO;
using System.Text;

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

            pipe.Connect(2000);

            using var writer = new StreamWriter(pipe, Encoding.UTF8);

            writer.WriteLine("SCAN");
            writer.Flush();
        }
        catch
        {
            MessageBox.Show("QSight가 실행되지 않았습니다.");
        }
    }
}