
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel.DataAnnotations;
using MonitoringWeb.Models;

namespace MonitoringWeb
{
    public class GroupOfSensorsTemp
    {
        public Dictionary<string, int> Dict { get; private set; }
        public SensorsTemp[] Sensors { get; private set; }
        public int Count { get; private set; }
        public string Request { get; private set; }

        public GroupOfSensorsTemp()
        {
            Dict = new Dictionary<string, int>();
            //string dataFilePath = @"C:\Users\n.goguev\source\repos\Monitoring\Monitoring\indexofallsensors.txt";
            //var index = File.ReadAllLines(dataFilePath);
            var index = File.ReadAllLines(@"config\indexofallsensors.txt");
            Sensors = new SensorsTemp[index.Length];
            for (var i = 0; i < index.Length; i++)
            {
                var tempString = index[i].Split("\t");
                var sensorsId = int.Parse(tempString[0])-1;
                Sensors[sensorsId] = new SensorsTemp {
                    Id = tempString[1],
                    Tempreture = 0,
                    Number = i,
                    Mount = tempString[2],
                    CRC = false,
                    LastGet = DateTime.Now
                };
                Dict.Add(tempString[1], sensorsId);

            }
        }

        public string[] GetDeviceNameAndCountBySSH(SshClient client)
        {
            var result = client.RunCommand("ls /sys/devices/w1_bus_master1");
            var stringResult = result.Result;
            var keyId = stringResult.Split("\n");
            Count = keyId.Length - 18;
            return keyId;
        }

        public void GetSSHRequest(int count, string[] keyId)
        {
            //cat / sys / devices / w1_bus_master1 / 28 - 051684a35eff / w1_slave
            var result = "cat ";
            for (var i = 0; i < count; i++)
            {
                result += "/sys/devices/w1_bus_master1/";
                result += keyId[i];
                result += "/w1_slave ";
            }
            Request = result;
        }

        public void GetDataBySSH(SshClient client, int count, string[] keyId)
        {
            PrintAndLogMessage("Data requested by SSH");
            var tempString = client.RunCommand(Request);
            PrintAndLogMessage("Data received by SSH");
            var resultTempString = tempString.Result;
            var number = 0;
            var temp = 0;
            var tempD = 0.0;
            var crc = false;
            var dateAndTime = DateTime.Now;
            for (var i = 0; i < resultTempString.Length; i++)
            {
                
                if (resultTempString[i] == '=')
                {
                    if (resultTempString.Substring(i + 4, 3) == "YES") crc = true;
                    else crc = false;
                    Sensors[Dict[keyId[number]]].CRC = crc;
                    for (var j = i+1; j < resultTempString.Length; j++)
                    {
                        if (resultTempString[j] == '=')
                        {
                            temp = Int32.Parse(resultTempString.Substring(j + 1, 5));
                            tempD = (double)temp / 1000;
                            Sensors[Dict[keyId[number]]].Tempreture = Math.Round(tempD,1);
                            Sensors[Dict[keyId[number]]].LastGet = dateAndTime;
                            number++;
                            i = j + 1;
                            break;
                        }
                    }    
                }
            }
        }

        //public void PrintAndLog()
        //{
        //    //using (StreamWriter file =
        //    //new StreamWriter(@"C:\Users\n.goguev\source\repos\Monitoring\Monitoring\Log.txt", true))
        //    using (StreamWriter file =
        //    new StreamWriter(Log.txt", true))
        //    {
        //        file.WriteLine("Start print " + DateTime.Now);
        //        foreach (var e in Sensors)
        //        {
        //            file.WriteLine(e.Number + 1 + "\t" + e.Tempreture + "\t" + e.CRC + "\t" + e.Mount);
        //        }
        //        file.WriteLine("End print " + DateTime.Now);
        //    }
        //    Console.WriteLine("Start print " + DateTime.Now);
        //    foreach (var e in Sensors)
        //    {
        //        Console.WriteLine(e.Number + 1 + "\t" + e.Tempreture + "\t" + e.CRC + "\t" + e.Mount);
        //    }
        //    Console.WriteLine("End print " + DateTime.Now);
        //}

        public void PrintAndLogMessage(string mes)
        {
            using (StreamWriter file =
            new StreamWriter(@"config\Log.txt", true))
            {
                file.WriteLine(mes + "\t" + DateTime.Now);
            }
            //Console.WriteLine(mes + "\t" + DateTime.Now);
        }

        public void WriteToSQLite()
        {
            // Delete all the records from the tables
            using (var context = new MonitoringWebContext())
            {
                foreach (var elem in context.Sensors)
                    context.Remove(elem);
                context.SaveChanges();
            }
            using (var context = new MonitoringWebContext())
            {
                foreach (var e in Sensors)
                {
                    if (e.Tempreture != 0)
                    {
                        var e2 = new SensorsTempData()
                        {
                            Id = e.Id,
                            CRC = e.CRC,
                            LastGet = e.LastGet,
                            Mount = e.Mount,
                            Number = e.Number,
                            Tempreture = e.Tempreture
                        };
                        context.SensorsData.Add(e2);
                    }
                    context.Sensors.Add(e);
                }
                context.SaveChanges();
            }
            PrintAndLogMessage("Write to SQL end");
            //// Select a previously inserted city record,
            //// insert a new person record referencing it,
            //// update a previously inserted person (specify the surname) 
            //using (var context = new Vs2015WinFormsEfcSqliteCodeFirst20170304ExampleContext())
            //{
            //    // Pay attention to the Include(city => city.People) part
            //    // simple context.Cities.Single(city => city.Name == "London"); used instead
            //    // would return the city but its .People list would be null.
            //    // Also make sure to handle cases when there are more or less than one records
            //    // meeting to the request conditions in the production code
            //    var london = context.Cities.Include(city => city.People).Single(city => city.Name == "London");
            //    var peter = new Person { Name = "Peter", City = london };
            //    var john = london.People.Single(person => person.Name == "John");
            //    john.Surname = "Smith";
            //    context.Add(peter);
            //    context.Update(john);
            //    context.SaveChanges();
            //}
        }

        public void RunSSH()
        {
            var connectionInfo = new PasswordConnectionInfo("10.0.68.20", "pi", "pass2025");
            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    if (client.IsConnected)
                    {
                        PrintAndLogMessage("SSH connection active");
                        var keyId = GetDeviceNameAndCountBySSH(client);
                        var countOfSensorsBeforeStart = Count;
                        while (true)
                        {
                            client.Disconnect(); // проверка
                            client.Connect(); // проверка
                            if (!client.IsConnected) break;
                            keyId = GetDeviceNameAndCountBySSH(client);
                            if (Count != countOfSensorsBeforeStart)
                            {
                                PrintAndLogMessage("Incorrect count");
                                countOfSensorsBeforeStart = Count;
                                Request = null;
                                //GroupSensors = new GroupOfSensorsTemp();
                                continue;
                            }
                            PrintAndLogMessage("Count of sensors : " + Count);
                            if (Request == null) GetSSHRequest(Count, keyId);
                            GetDataBySSH(client, Count, keyId);
                            //GroupSensors.PrintAndLog();
                            WriteToSQLite();
                        }
                        PrintAndLogMessage("SSH disconnect");
                    }
                    else PrintAndLogMessage("SSH connection NOTactive");
                }
                catch
                {
                //    PrintAndLogMessage("Unknown error");
                }
            }
            PrintAndLogMessage("SSH connection close"); //??
        }
    }
}
