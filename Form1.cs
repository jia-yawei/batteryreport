using System.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
namespace batteryreport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process process_cmd = new Process();
            process_cmd.StartInfo.FileName = "cmd.exe";//进程打开文件名为“cmd”
            process_cmd.StartInfo.RedirectStandardInput = true;//是否可以输入
            process_cmd.StartInfo.RedirectStandardOutput = true;//是否可以输出
            process_cmd.StartInfo.UseShellExecute = false;//是否使用操作系统shell启动
            process_cmd.StartInfo.CreateNoWindow = true;//是否在新窗口中启动进程
            process_cmd.Start();//启动进程
            process_cmd.StandardInput.WriteLine("powercfg /batteryreport");//输入命令
            Thread.Sleep(1000);//等待1秒   
            //获取电池报告的路径，这个路径在当前exe文件的同级目录下
            string batteryreport_path = System.IO.Directory.GetCurrentDirectory() + "\\battery-report.html";
            //使用HtmlAgilityPack解析html文件 
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(batteryreport_path);
            //获取电池报告的表格,在html文件第二个表格
            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[2]");
            //对表格进行处理，将表格中第4、6、7行，第2列的数据提取出来，依次放到dataGridView1第1行
            int i = 0;
            int k = 0;
            foreach (HtmlNode row in table.SelectNodes("tr"))
            {
                if (i == 4 || i == 6 || i == 7)
                {
                    int j = 0;
                    foreach (HtmlNode cell in row.SelectNodes("td"))
                    {
                        if (j == 1)
                        {
                            dataGridView1.Rows[0].Cells[k].Value = cell.InnerText;
                        }
                        j++;
                    }
                    k++;
                }
                i++;

            }

            //将第一列的值减去第二列的值，除以第一列的值，得到电池的损耗率             
            string str1 = dataGridView1.Rows[0].Cells[0].Value.ToString();
            string pattern1 = @"[^\d]";
            string result1 = Regex.Replace(str1, pattern1, "");
            string str2 = dataGridView1.Rows[0].Cells[1].Value.ToString();
            string pattern2 = @"[^\d]";
            string result2 = Regex.Replace(str2, pattern2, "");
            double result1_double = Convert.ToDouble(result1);
            double result2_double = Convert.ToDouble(result2);
            double result_double = (result1_double - result2_double) / result1_double;
            dataGridView1.Rows[0].Cells[3].Value = result_double.ToString("P");
            double result_ronsliang=1- result_double;
            dataGridView1.Rows[0].Cells[4].Value = result_ronsliang.ToString("P");
        }


    }
}
