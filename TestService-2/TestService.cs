using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Timers;


namespace TestService_2
{
    class TestService
    {
        private readonly System.Timers.Timer _timer;
        private List<EntryObject> Entries;

        static object LinksLock = new object();
        static object LinksLock1 = new object();
        static object LinksLock2 = new object();

        public TestService()
        {
            Entries = new List<EntryObject>();
            System.IO.DirectoryInfo di = new DirectoryInfo("Output");

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            _timer = new System.Timers.Timer(5000) { AutoReset = true };
            _timer.Elapsed += ToDo;

        }

        private void ToDo(object sender, ElapsedEventArgs e)
        {

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead("input.txt"))
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                var temp = new StringBuilder();


                string name = "", bookName = "", date = "", quantity = "";
                while ((line = streamReader.ReadLine()) != null)
                {

                    temp.Clear();
                    int check = 1;

                    for (int i = 0; i < line.Length; i++)
                    {
                        if (line[i] != ',')
                        {
                            temp.Append(line[i]);

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
                    foreach (EntryObject x in Entries)
                    {
                        if (x.Equals(Entry))
                            IfEntryAlreadyExists = true;

                    }
                    if (IfEntryAlreadyExists == false)
                    {
                        Entries.Add(Entry);
                        Task Task1 = Task.Factory.StartNew(() => UpdateDatabase(Entry));
                        Task Task2 = Task.Factory.StartNew(() => UpdateCustomerFile(Entry));
                        Task Task3 = Task.Factory.StartNew(() => UpdateInventory(Entry));


                    }


                }


            }


        }


        private void UpdateDatabase(EntryObject Entry)
        {
            System.Console.WriteLine("Updating db with: "+Entry.CustomerName + "  "+Entry.BookName+"  "+Entry.Quantity+"  "+Entry.OrderDate );

        }


        private void UpdateCustomerFile(EntryObject Entry)
        {

            lock (LinksLock1)
            {

                string[] Lines = new string[] { "Book Name: " + Entry.BookName, "Quantity: " + Entry.Quantity.ToString(), "Order Date: " + Entry.OrderDate + "\n" };


                File.AppendAllLines("Output\\" + Entry.CustomerName + ".txt", Lines);
            }
        }

        private void UpdateInventory(EntryObject Entry)
        {
            List<String> InventoryLines = new List<String>();

            lock (LinksLock)
            {


                using (StreamReader InventoryFile = new StreamReader("Inventory.txt"))
                {
                    String Line;
                    while ((Line = InventoryFile.ReadLine()) != null)
                    {
                        InventoryLines.Add(Line);
                        Line = "";
                    }
                    InventoryFile.Close();

                }


                String Temp = "";
                int i = 0, PreviousQuantity = 0;

           //     Console.WriteLine(InventoryLines.Count);
                foreach (String Line in InventoryLines.ToList())
                {
                    Temp = Line.Substring(0, Line.IndexOf(','));

                    // Console.WriteLine("Should be book name: "+Temp);
                    if (Entry.BookName == Temp)
                    {
                        Temp = Line.Substring(Line.IndexOf(',') + 1);
                        PreviousQuantity = Int16.Parse(Temp);
                        //          Console.WriteLine("Should be quantity: " + PreviousQuantity);

                        //        Console.WriteLine("Original Line" + InventoryLines[i] + '\n');
                        InventoryLines[i] = Entry.BookName + ',' + (PreviousQuantity - Entry.Quantity).ToString();

                        //         Console.WriteLine( "Should be full updated line "+ InventoryLines[i] + '\n');



                    }

                    i++;

                }



              //  Console.WriteLine("Inventory Lines Size in Write: " + InventoryLines.Count());


                // lock (LinksLock)
                //  {
                using (StreamWriter InventoryFileWriter = new StreamWriter("Inventory.txt"))
                {
                    foreach (String Line in InventoryLines)
                    {
                        InventoryFileWriter.WriteLine(Line);
                    }
                    InventoryFileWriter.Close();
                }
                
            }

                //   WriteToInventory(InventoryLines);


            }


        private static void WriteToInventory(List<String> InventoryLines)
        {
            Console.WriteLine("Inventory Lines Size in Write: "+InventoryLines.Count());
            

            lock (LinksLock)
            {
                using (StreamWriter InventoryFileWriter = new StreamWriter(@"C:\Users\ahmed.abdullah\Desktop\ServicesTest\TestService\Inventory.txt"))
                {
                    foreach (String Line in InventoryLines)
                    {
                        InventoryFileWriter.WriteLine(Line);
                    }
                    InventoryFileWriter.Close();
                }
            }

            

        }
        static void LineChanger(string newText, string fileName, int line_to_edit)
        {
            string[] arrLine = File.ReadAllLines(fileName);




            arrLine[line_to_edit - 1] = newText;
            File.WriteAllLines(fileName, arrLine);
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
