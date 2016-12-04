using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OneDriveRestAPI;
using OneDriveRestAPI.Model;
using Samples;
using File = OneDriveRestAPI.Model.File;

namespace MeCloud.Shared.OneDrive
{
    public class OneDriveCloud 
    {
        public async Task Run()
        {



            bool overWrite = true;
            OverwriteOption overwriteOption = OverwriteOption.DoNotOverwrite;
            if (overWrite) overwriteOption = OverwriteOption.Overwrite;


            // Initialize a new Client, this time by providing previously requested Access/Refresh tokens


            Options options = await GetAuthorizationToken();
            Client client = new Client(options);

            // Retrieve the root folder
            Folder rootFolder = await client.GetFolderAsync();
            IEnumerable<File> folderContent = await GetFolderContent(rootFolder, client);

            // Search for a file by pattern (e.g. *.docx for MS Word documents)
            //TODO: Uncomment the below when PR #5 is merged
            //var wordDocuments = await client.SearchAsync("*.docx");
            //Debug.WriteLine(string.Format("Found {0} Word documents", wordDocuments.Count()));

            //string fileToDownload = "";
            //var file = await DownloadFile(folderContent, client2, fileToDownload);


            foreach (string filePath in Directory.GetFiles(@"D:\dev\mecloud\Docs\TestFiles"))
            {
                string origFileName = Path.GetFileName(filePath);

                // Upload the file with a new name
                using (Stream fileStream = System.IO.File.Open(filePath,FileMode.Open))//_directory.OpenRead(filePath))
                {
                    int splitCount = 2;
                    Stream[] splitStream = new Stream[splitCount];
                    int[] byteLenghtOfSegment = new int[splitCount];
                    // First seg Len
                    byteLenghtOfSegment[0] = (int)Math.Ceiling((double) (fileStream.Length / (decimal)splitCount));
                    byteLenghtOfSegment[1] = (int)(fileStream.Length - byteLenghtOfSegment[0]);
                    int fileOffset = 0;

                    Stream ss = new MemoryStream();
                    using (BinaryReader reader = new BinaryReader(fileStream))
                    using (BinaryWriter writer = new BinaryWriter(ss))
                    {
                        writer.Write(reader.ReadBytes(byteLenghtOfSegment[0]));

                        await client.UploadAsync(rootFolder.Id, writer.BaseStream, origFileName + "1", overwriteOption);

                        writer.Flush();

                        writer.BaseStream.SetLength(0);

                        writer.Write(reader.ReadBytes(byteLenghtOfSegment[1]));

                        await client.UploadAsync(rootFolder.Id, writer.BaseStream, origFileName + "2", overwriteOption);
                    }


                    //BufferedStream bufferedStream = new BufferedStream(fileStream);
                    //bufferedStream.Write();

                    //splitStream[0].Write(fileStream.Read());
                    //// make await
                    //BitArray byteLength;
                    //fileStream.CopyTo(splitStream[0], (int) byteLength[0]);
                    //fileStream.Write(splitStream[0].,fileOffset, (int) byteLength[0]);

                    Debug.WriteLine("Uploading: " + origFileName);
                   //await client.UploadAsync(rootFolder.Id, fileStream, origFileName, overwriteOption);
                }

                string newFileName1 = origFileName + "1";
                string path = Path.GetDirectoryName(filePath);//_directory.GetDirectoryName(filePath);
                // Refresh Index on Server in root
                // TODO jpatel 5.21.15: Test exceptions by commenting out!
                folderContent = await GetFolderContent(rootFolder, client);
                await DownloadFile(folderContent, client, origFileName + "1", path, newFileName1);


                string newFileName2 = origFileName + "2";
                path = Path.GetDirectoryName(filePath);//_directory.GetDirectoryName(filePath);
                // Refresh Index on Server in root
                // TODO jpatel 5.21.15: Test exceptions by commenting out!
                folderContent = await GetFolderContent(rootFolder, client);
                await DownloadFile(folderContent, client, origFileName + "2", path, newFileName2);
                MergeFiles(path + "\\" + newFileName1, path + "\\" + newFileName2, path + "\\" + origFileName + "New.jpg");



            }
        }

        public void MergeFiles(string newFileName1, string newFileName2, string outPutFile)
        {
            try
            {

                using (FileStream fileStreamOut = System.IO.File.OpenWrite(outPutFile))
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(newFileName1))
                    {
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            fileStreamOut.WriteByte((byte) fileStream.ReadByte());
                        }


                        //Stream ss = new MemoryStream();

                        //using (BinaryReader reader = new BinaryReader(fileStream))

                        //using (BinaryWriter writer = new BinaryWriter(ss))

                        //{

                        // writer.Write(reader.ReadBytes((int) fileStream.Length));

                        // WriteToEndFile(outPutFile, writer);
                    }

                    using (FileStream fileStream = System.IO.File.OpenRead(newFileName2))
                    {
                        for (int i = 0; i < fileStream.Length; i++)
                        {
                            fileStreamOut.WriteByte((byte) fileStream.ReadByte());
                        }
                    }
                }
            }
            catch
                (Exception ex)
            {
                throw;
            }
        }

        public
            void WriteToEndFile(string outPutFile, BinaryWriter writer)
        {

            using (FileStream fileStream = System.IO.File.OpenWrite(outPutFile))
            {
                // Write the data to the file, byte by byte. 
                for (int i = 0; i < writer.BaseStream.Length; i++)
                {
                    fileStream.WriteByte((byte) writer.BaseStream.ReadByte());
                }

                // Set the stream position to the beginning of the file.
                fileStream.Seek(0, SeekOrigin.Begin);

            }
        }


        public  async Task<Options> GetAuthorizationToken()
        {
            Options options = new Options
            {
                ClientId = "0000000048152A56",
                ClientSecret = "PhQ7Z62wSxw6OvwvCfDWG2zKHlbLUCSt",
                CallbackUrl = "https://login.live.com/oauth20_desktop.srf",

                AutoRefreshTokens = true,
                PrettyJson = false,
                ReadRequestsPerSecond = 2,
                WriteRequestsPerSecond = 2
            };

            // Initialize a new Client (without an Access/Refresh tokens
            Client client = new Client(options);

            // Get the OAuth Request Url
            // var authRequestUrl = client.GetAuthorizationRequestUrl(new[] { Scope.Basic, Scope.Signin, Scope.SkyDrive, Scope.SkyDriveUpdate });

            // Navigate to authRequestUrl using the browser, and retrieve the Authorization Code from the response
            string authCode = await GetAuthCode(client, new[] { Scope.Signin, Scope.Basic, Scope.SkyDrive, Scope.SkyDriveUpdate, Scope.Photos });

            // Exchange the Authorization Code with Access/Refresh tokens
            UserToken token = await client.GetAccessTokenAsync(authCode);

            options.AccessToken = token.Access_Token;
            options.RefreshToken = token.Refresh_Token;

            //var auth=await client.GetAccessTokenAsync()
            // Get user profile
            User userProfile = await client.GetMeAsync();
            Debug.WriteLine("Name: " + userProfile.Name);
            return options;

        }


        public  async Task<File> DownloadFile(IEnumerable<File> folderContent, Client client, string fileToDownload, string fileOutPath, string fileDownloadName)
        {
            // Find file to download
            //File file = folderContent.FirstOrDefault(x => x.Type == File.FileType && x.Name == fileToDownload);

            Debug.WriteLine("Downloading {0} to {1} named: {2}", fileToDownload, fileOutPath, fileDownloadName);
            // Find file to download
            File file = folderContent.FirstOrDefault(x => x.Type != Folder.FolderType && x.Name == fileToDownload);
            if (file == null) Debug.WriteLine("File not found {0}", fileToDownload);
            // Download file to a temporary local file
            
            using (Stream fileStream = System.IO.File.Open(fileOutPath + "//" + fileDownloadName,FileMode.Create))
            {
                Stream contentStream = await client.DownloadAsync(file.Id);
                await contentStream.CopyToAsync(fileStream);
            }
            TaskCompletionSource<File> completion = new TaskCompletionSource<File>();
            completion.SetResult(file);
            Debug.WriteLine("Completed");
            return file;
      }

        public static async Task<IEnumerable<File>> GetFolderContent(Folder rootFolder, Client client)
        {
            Debug.WriteLine("Root Folder: {0} (Id: {1})", rootFolder.Name, rootFolder.Id);

            // Retrieve the content of the root folder
            IEnumerable<File> folderContent = await client.GetContentsAsync(rootFolder.Id);
            foreach (File item in folderContent)
            {
                Debug.WriteLine("\tItem ({0}: {1} (Id: {2})", item.Type, item.Name, item.Id);
            }
            return folderContent;
        }

        private Client _client = null;
        private object _directory;
        private Scope[] _scopes;

        public Task<string> GetAuthCode(Client client, Scope[] scopes)
        {
            TaskCompletionSource<string> completion = new TaskCompletionSource<string>();
            string code = null;
            code = "Ma20d089b-8686-ac46-0774-3cd4beda6ede";
            _client = client;
            _scopes = scopes;
           // var result = GetAuthCode(client, scopes);
           ////var result=  _getAuthCode(this);
            

            //var authUriString = string.Format("https://login.live.com/oauth20_authorize.srf?client_id={0}&scope={1}&response_type=code&redirect_uri={2}", clientId, Uri.EscapeDataString(scopesString), Uri.EscapeDataString(redirectUri));
            string authUriString = client.GetAuthorizationRequestUrl(scopes);

            BrowserWindow browser = new BrowserWindow();
            browser.Navigating += (sender, eventArgs) => Debug.WriteLine("Navigating: " + eventArgs.Uri);
            browser.Navigated += (sender, eventArgs) =>
            {
                /*
                 * Navigating: https://login.live.com/oauth20_authorize.srf?client_id=00000000480FBA5F&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf&scope=wl.signin wl.basic wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7
                 * Navigated: https://login.live.com/oauth20_authorize.srf?client_id=00000000480FBA5F&redirect_uri=https:%2F%2Flogin.live.com%2Foauth20_desktop.srf&scope=wl.signin wl.basic wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7
                 * Navigating: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive

                 * Navigated: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive
                 * Navigating: https://account.live.com/Consent/Update?ru=https://login.live.com/oauth20_authorize.srf%3flc%3d1033%26client_id%3d00000000480FBA5F%26redirect_uri%3dhttps%253A%252F%252Flogin.live.com%252Foauth20_desktop.srf%26scope%3dwl.signin%2520wl.basic%2520wl.skydrive%26response_type%3dcode%26display%3dwindesktop%26locale%3den-US%26state%3d%26theme%3dwin7%26mkt%3dEN-US%26scft%3dCtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%2524&mkt=EN-US&uiflavor=windesktop&id=279469&client_id=00000000480FBA5F&scope=wl.signin+wl.basic+wl.skydrive

                 * Navigating: https://login.live.com/oauth20_authorize.srf?lc=1033&client_id=00000000480FBA5F&redirect_uri=https:%2f%2flogin.live.com%2foauth20_desktop.srf&scope=wl.signin+wl.basic+wl.skydrive&response_type=code&display=windesktop&locale=en-US&state=&theme=win7&mkt=EN-US&scft=CtXDfhkaGJ68YBd6T1M!t4Qm3mQ31srlcyMC3z7hrYSNVo6nVNA0HXdY4BF8JWZDQYLjyoldXbRA3zPqAmhyEUDnX9BcDwStjFHkBQ2J7I!NqdNbiwXMngTKIt4C3fDQjccXbx3RMIE41mpmvSG6saU%24&res=success
                 * Navigating: https://login.live.com/oauth20_desktop.srf?code=4df80f16-2c7a-eb03-778e-f276c1dc95ee&lc=1033
                 * Navigated: https://login.live.com/oauth20_desktop.srf?code=4df80f16-2c7a-eb03-778e-f276c1dc95ee&lc=1033
                 */
                string uri = eventArgs.Uri.OriginalString;
                if (uri.Contains("code="))
                {
                    code = uri.Split(new[] { "code=" }, StringSplitOptions.None)[1];
                    code = code.Split(new[] { "&lc=" }, StringSplitOptions.None)[0];
                    Debug.WriteLine("Authorized: " + code);
                    browser.Close();
                }
                else
                {
                    Debug.WriteLine("Navigated: " + eventArgs.Uri);
                }
            };

            browser.Show(authUriString);
            completion.SetResult(code);

            return completion.Task;
        }

        public string GetAuthorizationRequestUrl()
        {
           return _client.GetAuthorizationRequestUrl(_scopes);
        }
    }
}
