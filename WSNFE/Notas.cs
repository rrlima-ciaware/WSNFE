using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Net;

namespace WSNFE
{
    public class Notas
    {

        private string repositorio;
        private string usuario;
        private string senha;

        private void buscarDadosAcesso()
        {
            string[] parametros = new string[3];
            Criptografia cripto = new Criptografia();

            using (var sr = new StreamReader(@"C:\inetpub\wwwroot\acesso.txt"))
            {
                int i = 0;
                while (!sr.EndOfStream)
                {
                    parametros[i] = sr.ReadLine();
                    i++;
                }
            }

            repositorio = parametros[0];
            int ini = repositorio.IndexOf(":") + 1;
            int fim = repositorio.Length - ini;
            repositorio = cripto.Decrypt(repositorio.Substring(ini, fim));

            usuario = parametros[1];
            ini = usuario.IndexOf(":") + 1;
            fim = usuario.Length - ini;

            usuario = usuario.Substring(ini, fim);

            if (usuario != "")
                usuario = cripto.Decrypt(usuario);

            senha = parametros[2];
            ini = senha.IndexOf(":") + 1;
            fim = senha.Length - ini;

            senha = senha.Substring(ini, fim);

            if (senha != "")
                senha = cripto.Decrypt(senha); 
         

        }


        public string buscarNota(int numPedido)
        {
            //Busca o XML
            try
            {
                buscarDadosAcesso(); 
                string nomArquivo = String.Format("{0:0000000000}", numPedido) + ".xml";
                FileStream stream = new FileStream(repositorio + nomArquivo.ToUpper(), FileMode.Open);
                
                //MemoryStream stream = buscarNotaFTP(numPedido);
                XmlSerializer desserializador = new XmlSerializer(typeof(TNfeProc));
                
                TNfeProc np = (TNfeProc)desserializador.Deserialize(stream);
                var serializer = JsonConvert.SerializeObject(np);

                stream.Close();
                stream.Dispose();
                return serializer;

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        
        private MemoryStream buscarNotaFTP(int numPedido)
        {
            //Buscar arquivos no FTP

            //Adiciona zeros a esquerda 
            string nomArquivo = String.Format("{0:0000000000}", numPedido) + ".xml";
            
            //Cria comunicação com o servidor
            //definindo o arquivo para download
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create("ftp://10.165.50.35/" + nomArquivo.ToUpper());
            //Define que a ação vai ser de download
            request.Method = WebRequestMethods.Ftp.DownloadFile;
            //Credenciais para o login (usuario, senha)
            request.Credentials = new NetworkCredential("menuserhe", "HE08cia");
            //modo passivo
            request.UsePassive = true;
            //dados binarios
            request.UseBinary = true;
            //setar o KeepAlive para true
            request.KeepAlive = true;

            //criando o objeto FtpWebResponse
            FtpWebResponse response = (FtpWebResponse)request.GetResponse();
            //Criando a Stream para ler o arquivo
            Stream responseStream = response.GetResponseStream();

            //Jogar arquivo do servidor ftp para o MemoryStream
            MemoryStream memoryStream = new MemoryStream();
            byte[] inputBuffer = new byte[65535];
            int readAmount;
            while ((readAmount = responseStream.Read(inputBuffer, 0, inputBuffer.Length)) > 0)
                memoryStream.Write(inputBuffer, 0, readAmount);

            responseStream.Close();
            responseStream.Dispose();

            //Lê o conteúdo do MemoryStream
            memoryStream.Position = 0;
            StreamReader streamReader = new StreamReader(memoryStream);
            string xmlOutput = streamReader.ReadToEnd();

            memoryStream.Close();
            memoryStream.Dispose();

            //Converte o conteúdo string para stream, necessário esse processo para gerar o stream que será passado para serialização
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(xmlOutput);
            writer.Flush();
            stream.Position = 0;

            response.Close();
            return stream;
        
            
        }



    }
}