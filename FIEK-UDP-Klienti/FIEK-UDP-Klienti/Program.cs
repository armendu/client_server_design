using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace FIEK_UDP_Klienti
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Klienti";
            MetodatShtese objMetodShtese = new MetodatShtese();     //krijimi i objektit te klases Metodatshtese 
            Console.ForegroundColor = ConsoleColor.Blue; 
            Console.WriteLine("-- Klienti --\n");
            string localhost = "127.0.0.1";     //localhost eshte hosti i nenkuptuar
        Fillimi:    //perdorimin etiken per tu kthyr perseri ne kete pjese te kodit             
            int porti = 7000;   //porti i nenkuptuar
            
            Console.ForegroundColor = ConsoleColor.White;
            IPEndPoint ip = new IPEndPoint(IPAddress.Parse(localhost), porti);  //krijimi i objektit te IP PIKES FUNDORE dhe inicializimi i saj
            Socket socketKlienti = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //krijimi i objektit soketi & inicializimi i tij           

            try     //try,catch perdoren per gjetjen e gabimeve
            {
                socketKlienti.Connect(ip);  //Lidhu(connect) me ip
            }
            catch (SocketException ErrorEX) //kalon ne kete pjese te kodit nese deshton TRY(merr vleren false)
            {
                Console.WriteLine("Dështoj lidhja me serverin, ka ndodhë ky gabim (" + ErrorEX + ")\n");  //shfaqja e gabimit qe ka ndodhur
                                                     
            }
            //operacionet qe mund ti shkruaj klienti:
            Console.Write("Ju lutemi shkruani 'ndrroip' per tu lidhur me nje server tjeter,'exit' per ta mbyllur aplikacionin ose shkruaj njerin nga operacionet: \nIP,PORT,ZANORE,PRINTO,HOST,TIME,KENO,KONVERTO,FAKTORIEL,KONVERTODY,PMP: "); 
            Console.ForegroundColor = ConsoleColor.Yellow;
            string strTekstiHyres = Console.ReadLine().Trim();//lexo shkrimin,largo hapesirat para dhe mbrapa

            int dyHpasira = strTekstiHyres.IndexOf("  "); //gjene indeksin nese ka ndonje karakter 2 zbrastira apo 3 apo 4            
            if (strTekstiHyres == "") //nese klienti shtyp vetem enter  
            {
                Console.ForegroundColor = ConsoleColor.Red;      //si lajmerim
                Console.WriteLine("Ju lutem shkruani nje komande!\n");
                goto Fillimi;   //detyrohet te shkoje te etiketa 'Fillimi'
            }
            else if (dyHpasira > 0) //nese klienti shkruan 2hapsira t'i kerkohet edhe njehere komanda 
            {
                int Numrimi = 2; //nje variabel ndihmese
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Teksti juaj ka {0} zbrastira se bashku. Ju lutemi shkruani me vetem 1(nje) zbrastire\n", Numrimi); //ilustrimi i shtypjes se formatizuar
                goto Fillimi;   //detyrohet te shkoje te etiketa 'Fillimi'
            }
            else if (strTekstiHyres.ToLower() == "exit") { return; } //nese perdoruesi deshiron te mbylle aplikacionin
            else if (strTekstiHyres.ToLower() == "ndrroip")          //klienti duhet te zgjedh ip e hostit ne te cilin do te lidhet
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Shkruani IP adresen e serverit: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string IPAdresa = Console.ReadLine();  //string per lexim te IP adreses qe do te jep shfrytezuesi
                IPAddress address;
                if (IPAddress.TryParse(IPAdresa, out address))     //validimi i IP adreses      
                    localhost = IPAdresa;
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; //si lajmerim
                    Console.Write("Ju keni dhënë IP adresën në format të gabuar!\n\n");
                    goto Fillimi;
                }
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("Shkruani portin e serverit: ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                string sPorti = Console.ReadLine(); //stringu sPorti qe do te jepet nga shfrytezuesi
                if (sPorti == "") { }   //nese nuk jep asgje, ateher porti do te jet ai i nenkuptuar
                else porti = Convert.ToInt32(sPorti);   //e modifikojme portin, duke e konvertuar ne int stringun sPorti
            }
            //metodat shtese
            else if ((strTekstiHyres.ToLower().IndexOf("konvertody") == 0))
            {

                Console.WriteLine(objMetodShtese.KONVERTODY(strTekstiHyres) + "\n");
                goto Fillimi;
            }
            else if ((strTekstiHyres.ToLower().IndexOf("pmp") == 0))
            {

                Console.WriteLine(objMetodShtese.PMP() + "\n");
                goto Fillimi;
            }

            //dergimi i kerkeses permes socketKlientit
            socketKlienti.Send(Encoding.ASCII.GetBytes(strTekstiHyres));              


            //pranimi i pergjigjes nga Serveri
            byte[] data = new byte[128];    //bajt array max 128
            try
            {
                int recivedDataLength = socketKlienti.Receive(data);    //gjatesia e mesazhit
                string edhenangaserveri = Encoding.ASCII.GetString(data, 0, recivedDataLength); //ruajtja e rezultatit te kthyer nga serveri ne stringun 'edhenangaserveri'
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Serveri: " + edhenangaserveri + "\n");//pergjigja nga serveri
                goto Fillimi;
            }
            catch (SocketException ErrorExep)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write("\nGjatë komunikimit me Server ka ndodhë ky gabim: \n");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ErrorExep);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nShtypni ENTER per te provuar edhe nje here ose shkruaj EXIT per te mbyllur aplikacionin: ");
                string perfundimi = Console.ReadLine();//i ipet mundesia te nderpritet programi
                if (perfundimi == "")
                {
                    goto Fillimi;
                }
                else if (perfundimi.ToLower() == "exit") { return; }
                
            }
            socketKlienti.Shutdown(SocketShutdown.Both);    //Nderprerja e dergimi,pranimit nepermes socketKlientit
            socketKlienti.Close();  //Mbyllja e 'scoketKlienti' lidhjes
        } 
        

        //metodat shtese
        class MetodatShtese
        {
            public string KONVERTODY(string Teksti) //metod per konvertime
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
                    case "mti": Rezultati = Vlera * 39.370; break;//metertoinch
                    case "itm": Rezultati = Vlera / 39.370; break;//inchtometer
                    case "cti": Rezultati = Vlera * 0.39370; break;//cmtoinch
                    case "itc": Rezultati = Vlera / 0.39370; break;//inchtocm
                    case "ktm": Rezultati = Vlera * 0.62137; break;//kmtomiles
                    case "mtk": Rezultati = Vlera / 0.62137; break;//milestokm
                    case "ftm": Rezultati = Vlera / 3.2808; break;//feettometer
                    case "mtf": Rezultati = Vlera * 3.2808; break;//metertofeet
                    default: return "Vlerat per konvertim nuk jane dhene si duhet!\n";
                }
                return "Rezultati: " + Rezultati + "\n"
                    + "-----------------------------------------------------------------"; //kthimi i rezultatit
            }
            public string PMP()
            {
                int n1, n2;
                Random numrat = new Random();
                n1 = numrat.Next(1, 9);
                n2 = numrat.Next(1, 9);
                int y = pmp(n1, n2);

                return "Pjestuesi me i madh i perbashket per numrat: " + n1 + "," + n2 + " eshte: " + y;
            }

            public static int pmp(int n1, int n2)
            {
                if (n2 == 0)
                    return n1;
                else
                {
                    return pmp(n2, n1 % n2);
                }
            }

        }
    }
}
