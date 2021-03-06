﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;//na jep mundesi te perdorim Threads

namespace FIEK_TCP_Serveri
{
    class Program
    {

        static List<string> ListaMeKlient = new List<string>();
        static void Main(string[] args)
        {
            Console.Title = "Serveri";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("-- Serveri --\n");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Serveri eshte duke pritur per nje klient");

            Socket socketiMirseardhes;
            Socket serverSocket;      //krijimi i objekti i soketit
            IPEndPoint ip;  //krijimi i objkti i IPEndPoint
        Fillimi:

            ip = new IPEndPoint(IPAddress.Any, 7000);//Krijimi i nje pike fundore me ip dhe port numer
            socketiMirseardhes = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //Krijimi i nje socketi dhe inicilizimi qe punon me tcp
            socketiMirseardhes.Bind(ip); //krijo lidhjen me ate ip
            socketiMirseardhes.Listen(10); //degjon deri ne 10 klient
            serverSocket = socketiMirseardhes.Accept(); //ben pranimin e klienteve

            KlientiRi klienti = new KlientiRi(serverSocket); //krijimi i objetit klienti te klases Klienti i ri
            socketiMirseardhes.Close();//Mbyllet socketiMirseardhes
            goto Fillimi;
        }

        class KlientiRi //klasa per klientet e ri
        {
            Socket serverSocket; //krijimi i objekti i soketit
            Metodat objMetoda = new Metodat();  //krijimi i objektit te 'Metodat'
            IPEndPoint objEndpoint; //krijimi i objkti i Endpoint(pika e fundit)

            public KlientiRi(Socket serverSocket)
            {
                this.serverSocket = serverSocket; 
                this.objEndpoint = (IPEndPoint)serverSocket.RemoteEndPoint;//Marrja e ip dhe portit
                Thread cThread = new Thread(Komunikimi); //krijon nje thread te ri per komunikim
                if (!ListaMeKlient.Contains(objEndpoint.Address.ToString()))
                    ListaMeKlient.Add(objEndpoint.Address.ToString());  //shtimi ne liste te klientit
                cThread.Start(); //nis threadin per ate klient
            }

            public void Komunikimi()
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Serveri u lidh me klientin {0} me nr: {1} ne portin {2}", objEndpoint.Address, (ListaMeKlient.IndexOf(objEndpoint.Address.ToString()) + 1).ToString(), objEndpoint.Port); //info rreth klientit
                
                try
                {
                    byte[] eDhenaPranuese = new byte[128];//byte array nga serveri
                    int gjatsia = serverSocket.Receive(eDhenaPranuese);//Leximi i gjatesise mesazhit nga klienti permes serversocket
                    string TeDhenat = Encoding.ASCII.GetString(eDhenaPranuese, 0, gjatsia);//ruajtja e rezultatit te kthyer nga serveri ne stringun 'TeDhenat'

                    string pergjigjja = "";     //string ku do te ruajme te dhenat
                    TeDhenat = TeDhenat.Trim();
                    if (TeDhenat.ToLower() == "ip")
                        pergjigjja = objMetoda.IP(objEndpoint);//dergon tek IP metoda
                    else if (TeDhenat.ToLower() == "port")
                        pergjigjja = objMetoda.PORT(objEndpoint);//dergon tek Port metoda
                    else if (TeDhenat.ToLower() == "time")
                        pergjigjja = objMetoda.TIME();//dergon tek Time metoda
                    else if (TeDhenat.ToLower() == "host")
                        pergjigjja = objMetoda.HOST();//dergon tek Host metoda
                    else if (TeDhenat.ToLower().IndexOf("zanore") == 0)
                        pergjigjja = objMetoda.ZANORE(TeDhenat.ToLower());//dergon tek TeDhenat metoda
                    else if (TeDhenat.ToLower().IndexOf("printo") == 0)
                        pergjigjja = objMetoda.PRINTO(TeDhenat.ToLower());//dergon tek Printo metoda
                    else if (TeDhenat.ToLower().IndexOf("konverto") == 0)
                        pergjigjja = objMetoda.KONVERTO(TeDhenat.ToLower());//dergon tek Konverto metoda
                    else if (TeDhenat.ToLower() == "keno")
                        pergjigjja = objMetoda.KENO();//dergon tek Keno metoda
                    else if (TeDhenat.ToLower().IndexOf("faktoriel") == 0)
                        pergjigjja = objMetoda.FAKTORIEL(TeDhenat.ToLower());//dergon tek Faktoriel metoda
                    else
                    {
                        pergjigjja = "Kerkesa eshte e padefinuar mire!\n";//nese deshton
                    }

                    byte[] PergjigjaeKthyer = new byte[128];    //bajt array me 128max per ruatje te pergjigjes       
                    PergjigjaeKthyer = Encoding.ASCII.GetBytes(pergjigjja);//pergjigjen duhet ta kthejm ne bajta
                    serverSocket.SendTo(PergjigjaeKthyer, objEndpoint);//dhe pastaj ja dergojm serverit

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Klienti: " + TeDhenat);//shtypet kerkesa e klientit
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine("Serveri: " + pergjigjja); //shtypet pergjigja e serverit(te gjitha shtypen ne klient)
                }
                catch (Exception Error) //per te dhene gabimet, do shfaqet mesazhi
                {
                    Console.WriteLine("Gabim: " + Error); 
                }
            }
        }

        //klase e re me metodat qe do te mund ti perdor klienti
        class Metodat
        {
            public string IP(EndPoint k)
            {   //kthimi i ip adreses
                string[] ndarja = (k.ToString()).Split(':');//ndarje ne ':'
                return "IP adresa e klientit eshte: " + ndarja[0] + "\n"
                    + "-----------------------------------------------------------------";//ip eshte pjesa e pare e ndarjes
            }

            public string PORT(EndPoint k)
            {   //kthimi i portit
                string[] ndarja = (k.ToString()).Split(':');
                return "Klienti eshte duke perdorur portin: " + ndarja[1] + "\n"
                    + "-----------------------------------------------------------------";//pjesa e dyte e ndarjes eshte porti
            }

            public string ZANORE(string Teksti)
            {   //metode per numerimin e zanoreve
                int Hapsira = Teksti.IndexOf(' ');//gjej vendin e hapsires
                string text = Teksti.Substring(Hapsira + 1, Teksti.Length - Hapsira - 1);//Merr tekstin qe ka shkruar klienti

                int Totali = text.Count(c => "aeiouy".Contains(Char.ToLower(c))); //numero nese ne text ka ndonje nga kto shkronja   
                return "Teksti i pranuar permban " + Totali + " zanore\n"
                    + "-----------------------------------------------------------------";  //rezultati me nr te zanoreve      
            }

            public string PRINTO(string Teksti)
            {    //metode per printimin te tekstit
                int Hapsira = Teksti.IndexOf(' ');  //gjej hapsiren
                string text = Teksti.Substring(Hapsira + 1, Teksti.Length - Hapsira - 1);   //Merr tekstin qe ka shkruar klienti
                return "" + text + "\n"
                    + "-----------------------------------------------------------------";//printimi i atij teksti
            }

            public string HOST()
            {   //shfaqjen e hostit
                if (Dns.GetHostName() == null) //ne rast se nuk mund te gjindet emrin i hostit
                    return "Emri i hostit nuk dihet!\n";
                else
                    return "Emri i hostit eshte " + Dns.GetHostName() + "\n"
                        + "-----------------------------------------------------------------";
            }

            public string TIME()
            {   //kthimi i kohes
                return "Ora eshte :" + DateTime.Now + "\n"
                    + "-----------------------------------------------------------------";
            }

            public string KENO()
            {   //kthimi i 20numrave random
                List<int> nrKeno = new List<int>();//list qe do te mbushet me vone
                Random rnd = new Random();//krijimi i objektit random 
                string x = "";//x per fillim i zbrazt
                do
                {
                    int numeri = rnd.Next(1, 81);//nr. do te jete random mes 1 dhe 80
                    nrKeno.Add(numeri);//shtohet ai nr. ne kete liste
                }
                while (nrKeno.Count < 20);//kryej operacionin me larte 20 here

                for (int i = 0; i < 19; i++)//shtypja e anetare
                {
                    x += nrKeno[i] + ",";
                }
                x = x + nrKeno[19];//anetari i fundit pa presje
                return x + "\n"
                    + "-----------------------------------------------------------------";//shfaq numrat
            }

            public string FAKTORIEL(string Teksti)
            {   //gjetja e faktorielit
                int Hapsira = Teksti.IndexOf(' ');//gjej hapsiren
                string text = Teksti.Substring(Hapsira + 1, Teksti.Length - Hapsira - 1);//Merr tekstin qe ka shkruar klienti

                double numri = 0;
                double Rezultati = 1;
                try
                {
                    numri = Convert.ToDouble(text);//kthe tekstin ne nr
                }
                catch (Exception Gabimi)
                {
                    return "Gabim ne konvertim numri. Ju lutem shkruani nje numer \n" + Gabimi;
                }
                for (int i = 1; i <= numri; i++) { Rezultati *= i; }//gjetja e faktorielit
                return Rezultati.ToString() + "\n"
                    + "-----------------------------------------------------------------";//kthimi i rezultatit
            }

            public string KONVERTO(string Teksti) //metod per konvertime
            {
                string[] eArdhura = Teksti.Split(' ');//e ndan tekstin hyres ne 3 pjese permes hapsires   
                int Vlera = 0; //vlera fillestare
                double Rezultati = 0; //rezultati ku do te ruhen llogaritjet
                try
                {
                    Vlera = Convert.ToInt32(eArdhura[2]); //te marre vlerat te konvertuara ne int
                }
                catch (Exception Gabimi)
                {
                    return "Formati i numrit nuk eshte ne rregull. Ka ndodhur gabimi: \n" + Gabimi;
                }

                switch (eArdhura[1])
                {   //teksti i dyte dduhet te jete njeri nga operacionet :
                    case "ftc": Rezultati = ((Vlera - 32) * (5.0 / 9)); break; //fahrenheittocelsius
                    case "ctf": Rezultati = Vlera * 9 / 5 + 32; break;//celsiustofahrenheit
                    case "ctk": Rezultati = Vlera + 273.15; break;//celsiustokelvin
                    case "ktf": Rezultati = Vlera * 9 / 5 - 459.67; break;//kelvintofahrenheit
                    case "ktc": Rezultati = Vlera - 273.15; break;//kelvintocelsius
                    case "ftk": Rezultati = (Vlera + 459.67) * 5 / 9; break;//fahrenheittokelvin
                    case "ptk": Rezultati = Vlera / 2.2046; break;//poundtokilogram
                    case "ktp": Rezultati = Vlera * 2.2046; break;//kilogramtopound
                    default: return "Vlerat per konvertim nuk jane dhene si duhet!\n";
                }
                return "" + Rezultati + "\n"
                    + "-----------------------------------------------------------------"; //kthimi i rezultatit
            }
        }
    }
}
