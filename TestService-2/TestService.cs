using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace TestService_2
{
    class TestService
    {
        private readonly Timer _timer;
        private List<EntryObject> Entries;
        //private List<string> CustomerNames;
        //private List<string> BookNames;
        //private List<int> Quantity;
        //private List<string> DateTime;
        public TestService()
        {
            Entries = new List<EntryObject>();
            System.IO.DirectoryInfo di = new DirectoryInfo("Output");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            _timer = new Timer(5000) { AutoReset = true };
            _timer.Elapsed += ToDo;

        }

        private void ToDo(object sender, ElapsedEventArgs e) { 

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("input.txt")) 
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize)) {
                    String line;
                    var temp = new StringBuilder();
               
                    
                    string name = "",bookName="",date="",quantity = "";
                    while ((line = streamReader.ReadLine()) != null)
                    {

                    temp.Clear();
                    int check = 1;
                    
                    for (int i = 0;i< line.Length; i++)
                        {
                        if (line[i] != ',')
                        {

                            // System.Console.WriteLine(temp);
                            temp.Append(line[i]);
                           //i++;
                        }
                        else
                        {
                            
                            if (check == 1)
                            {
                                name = temp.ToString();

                            }
                            else if (check == 2)
                            {
                                bookName = temp.ToString();
                            }
                            else if (check == 3)
                            {
                                quantity = temp.ToString();
                            }
                            else if (check == 4)
                            {
                                date = temp.ToString();
                            }
                            temp.Clear();
                            check++;
                        }

                    }
                    date = temp.ToString();

                    EntryObject Entry = new EntryObject
                    {
                        CustomerName = name,
                        BookName = bookName,
                        Quantity = Int16.Parse(quantity),
                        OrderDate = date
                    };
                    bool IfEntryAlreadyExists = false;
                    foreach(EntryObject x in Entries)
                    {
                        if (x.Equals(Entry))
                            IfEntryAlreadyExists = true;

                    }
                    if (IfEntryAlreadyExists==false)
                    {
                        Entries.Add(Entry);

                        string[] lines = new string[] { "Book Name: " + bookName, "Quantity: " + quantity.ToString(), "Order Date: " + date+"\n" };

                        File.AppendAllLines("Output\\" + name + ".txt", lines);
                    }
                    
                
            /*        string[] lines = new string[] { "Book Name: "+bookName,"Quantity: "+quantity.ToString(),"Order Date"+date};
                    
                    File.AppendAllLines(@"C:\Users\ahmed.abdullah\Desktop\ServicesTest\TestService\Output\"+name+".txt", lines);
            */

                }
        
                }


        }


        public void Start()
        {
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

    }
}
