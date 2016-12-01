using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SwaggerJavaClientFixer
{
    public partial class Form1 : Form
    {
        private const string KEY_APP = "SWAGGER_IO_FIXER";
        private const string KEY_APP_DEFAULT_FOLDER = "SWAGGER_IO_FIXER_DEFAULT_FOLDER";

        private bool _mShouldExit = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            _mShouldExit = true;
            Application.Exit();
        }

        private bool CheckIsReady()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(txtFolder.Text);
            if (dirInfo.Exists)
            {
                lblStatus.Text = "Status: READY TO PROCESS";
                return true;
            }
            else
            {
                lblStatus.Text = "Status: FOLDER DOES NOT EXIST!";
                return false;
            }
        }

        private void CacheFolder()
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.CreateSubKey(KEY_APP);
            key.SetValue(KEY_APP_DEFAULT_FOLDER, txtFolder.Text);
            key.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.SelectedPath = txtFolder.Text;
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFolder.Text = folderBrowserDialog1.SelectedPath;
                CacheFolder();
            }

            CheckIsReady();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (string name in Microsoft.Win32.Registry.CurrentUser.GetSubKeyNames())
            {
                if (name == KEY_APP)
                {
                    Microsoft.Win32.RegistryKey key =
                        Microsoft.Win32.Registry.CurrentUser.OpenSubKey(KEY_APP);
                    if (null != key)
                    {
                        txtFolder.Text = (string)key.GetValue(KEY_APP_DEFAULT_FOLDER);
                        CheckIsReady();
                        return;
                    }
                }
            }
            txtFolder.Text = Directory.GetCurrentDirectory();
            CheckIsReady();
        }

        private void FindAllChildren(DirectoryInfo folder, string extension, Action<FileInfo> action)
        {
            if (folder.Exists)
            {
                foreach (FileInfo fileInfo in folder.GetFiles("*.java"))
                {
                    action(fileInfo);
                }
                foreach (DirectoryInfo child in folder.GetDirectories())
                {
                    if (child.Name == ".")
                    {
                        continue;
                    }
                    FindAllChildren(child, extension, action);
                }
            }
        }

        private void SetStatus(string text)
        {
            Action action = () =>
                {
                    lblStatus.Text = text;
                };
            this.Invoke(action);
        }

        string GetNamedValue(int val)
        {
            string result = HumanFriendlyInteger.IntegerToWritten(val);
            return result.Replace(" ", "_");
        }

        private void ProcessFile(FileInfo fileInfo)
        {
            if (_mShouldExit)
            {
                return;
            }

            if (!fileInfo.Exists)
            {
                SetStatus(string.Format("Status: File does not exist: {0}", fileInfo.Name));
                return;
            }

            SetStatus(string.Format("Status: Reading: {0}", fileInfo.Name));
            string originalContent = string.Empty;
            string newContent = string.Empty;
            using (StreamReader reader = new StreamReader(fileInfo.FullName))
            {
                String previousLine = null;
                String line = null;
                do
                {
                    previousLine = line;
                    line = reader.ReadLine();

                    if (null != line)
                    {
                        originalContent += line;
                        originalContent += Environment.NewLine;
                    }

                    string replaceLine = null;

                    const string TOKEN_CALL = "import okhttp3.Call;";
                    const string TOKEN_OLD_CALL = "import com.squareup.okhttp.Call;";

                    const string TOKEN_CALLBACK = "import okhttp3.Callback;";
                    const string TOKEN_OLD_CALLBACK = "import com.squareup.okhttp.Callback;";

                    const string TOKEN_CREDENTIALS = "import okhttp3.Credentials;";
                    const string TOKEN_OLD_CREDENTIALS = "import com.squareup.okhttp.Credentials;";

                    const string TOKEN_HTTP_CLIENT = "import okhttp3.OkHttpClient;";
                    const string TOKEN_OLD_HTTP_CLIENT = "import com.squareup.okhttp.OkHttpClient";

                    const string TOKEN_FORM_ENCODING_BUILDER = "import okhttp3.FormBody;";
                    const string TOKEN_OLD_FORM_ENCODING_BUILDER = "import com.squareup.okhttp.FormEncodingBuilder;";

                    const string TOKEN_HEADERS = "import okhttp3.Headers;";
                    const string TOKEN_OLD_HEADERS = "import com.squareup.okhttp.Headers;";

                    const string TOKEN_HTTP_LOGGING = "import okhttp3.logging.HttpLoggingInterceptor;";
                    const string TOKEN_OLD_HTTP_LOGGING = "import com.squareup.okhttp.logging.HttpLoggingInterceptor;";

                    const string TOKEN_HTTP_LOGGING_LEVEL = "import okhttp3.logging.HttpLoggingInterceptor.Level;";
                    const string TOKEN_OLD_HTTP_LOGGING_LEVEL = "import com.squareup.okhttp.logging.HttpLoggingInterceptor.Level;";

                    const string TOKEN_HTTP_METHOD = "import okhttp3.internal.http.HttpMethod;";
                    const string TOKEN_OLD_HTTP_METHOD = "import com.squareup.okhttp.internal.http.HttpMethod;";

                    const string TOKEN_MEDIA_TYPE = "import okhttp3.MediaType;";
                    const string TOKEN_OLD_MEDIA_TYPE = "import com.squareup.okhttp.MediaType;";

                    const string TOKEN_MULTIPART_BUILDER = "import okhttp3.MultipartBody;";
                    const string TOKEN_OLD_MULTIPART_BUILDER = "import com.squareup.okhttp.MultipartBuilder;";

                    const string TOKEN_REQUEST = "import okhttp3.Request;";
                    const string TOKEN_OLD_REQUEST = "import com.squareup.okhttp.Request;";

                    const string TOKEN_REQUEST_BODY = "import okhttp3.RequestBody;";
                    const string TOKEN_OLD_REQUEST_BODY = "import com.squareup.okhttp.RequestBody;";

                    const string TOKEN_RESPONSE = "import okhttp3.Response;";
                    const string TOKEN_OLD_RESPONSE = "import com.squareup.okhttp.Response;";

                    const string TOKEN_RESPONSE_BODY = "import okhttp3.ResponseBody;";
                    const string TOKEN_OLD_RESPONSE_BODY = "import com.squareup.okhttp.ResponseBody;";

                    const string TOKEN_NAMESPACE_CALL = "okhttp3.Call";
                    const string TOKEN_OLD_NAMESPACE_CALL = "com.squareup.okhttp.Call";

                    const string TOKEN_NAMESPACE_INTERCEPTOR = "okhttp3.Interceptor";
                    const string TOKEN_OLD_NAMESPACE_INTERCEPTOR = "com.squareup.okhttp.Interceptor";

                    const string TOKEN_NAMESPACE_RESPONSE = "okhttp3.Response";
                    const string TOKEN_OLD_NAMESPACE_RESPONSE = "com.squareup.okhttp.Response";

                    // ApiClient.java
                    const string TOKEN_GET_CONNECT_TIMEOUT = "httpClient.connectTimeoutMillis()";
                    const string TOKEN_OLD_GET_CONNECT_TIMEOUT = "httpClient.getConnectTimeout()";

                    // ApiClient.java
                    const string TOKEN_SET_CONNECT_TIMEOUT = "httpClient.newBuilder().connectTimeout(connectionTimeout, TimeUnit.MILLISECONDS)";
                    const string TOKEN_OLD_SET_CONNECT_TIMEOUT = "httpClient.setConnectTimeout(connectionTimeout, TimeUnit.MILLISECONDS)";

                    // ApiClient.java
                    const string TOKEN_TYPE_FORM_ENCODING_BUILDER = "FormBody.Builder";
                    const string TOKEN_OLD_TYPE_FORM_ENCODING_BUILDER = "FormEncodingBuilder";

                    // ApiClient.java
                    const string TOKEN_TYPE_MULTIPART_BUILDER = "MultipartBody.Builder";
                    const string TOKEN_OLD_TYPE_MULTIPART_BUILDER = "MultipartBuilder";

                    // ApiClient.java
                    const string TOKEN_OK3_MULTIPART = "new MultipartBody.Builder().setType(MultipartBody.FORM)";
                    const string TOKEN_OK1_MULTIPART = "new MultipartBuilder().type(MultipartBuilder.FORM)";

                    // ApiClient.java
                    const string TOKEN_FAILURE_REQUEST = "public void onFailure(Call call, IOException e) {";
                    const string TOKEN_OLD_FAILURE_REQUEST = "public void onFailure(Request request, IOException e) {";

                    // ApiClient.java
                    const string TOKEN_ASYNC_RESPONSE = "public void onResponse(Call call, Response response) throws IOException {";
                    const string TOKEN_OLD_ASYNC_RESPONSE = "public void onResponse(Response response) throws IOException {";

                    // ApiClient.java
                    const string TOKEN_SET_SSL = "httpClient.newBuilder().sslSocketFactory(";
                    const string TOKEN_OLD_SET_SSL = "httpClient.setSslSocketFactory(";

                    // ApiClient.java
                    const string TOKEN_SET_HOSTNAME_VERIFIER = "httpClient.newBuilder().hostnameVerifier(";
                    const string TOKEN_OLD_SET_HOSTNAME_VERIFIER = "httpClient.setHostnameVerifier(";

                    // ProgressResponseBody.java
                    const string TOKEN_CONTENT_LENGTH = "public long contentLength() {";
                    const string TOKEN_OLD_CONTENT_LENGTH = "public long contentLength() throws IOException {";

                    // ProgressResponseBody.java
                    const string TOKEN_BUFFERED_SOURCE = "public BufferedSource source() {";
                    const string TOKEN_OLD_BUFFERED_SOURCE = "public BufferedSource source() throws IOException {";

                    // ref: http://stackoverflow.com/questions/34895397/not-able-to-import-com-squareup-okhttp-okhttpclient
                    // add lib: http://stackoverflow.com/questions/28465603/error-package-javax-annotation-does-not-exist-after-upgrade-to-lombok-1-16-2
                    // add lib: http://stackoverflow.com/questions/29237563/adding-joda-time-to-android-studio

                    // ref: https://github.com/square/okhttp/blob/master/CHANGELOG.md
                    // Form and Multipart bodies are now modeled. We've replaced the opaque FormEncodingBuilder
                    // with the more powerful FormBody and FormBody.Builder combo. Similarly we've upgraded
                    // MultipartBuilder into MultipartBody, MultipartBody.Part, and MultipartBody.Builder.

                    // ref: http://stackoverflow.com/questions/34930167/upload-dynamic-number-of-files-with-okhttp3

                    /*
                     * //httpClient.setConnectTimeout(connectionTimeout, TimeUnit.MILLISECONDS);
                     * httpClient.newBuilder().connectTimeout(connectionTimeout, TimeUnit.MILLISECONDS);
                     * 
                     * //httpClient.setSslSocketFactory(sslContext.getSocketFactory());
                     * httpClient.newBuilder().sslSocketFactory(sslContext.getSocketFactory());
                     * 
                     * //httpClient.setSslSocketFactory(null);
                     * httpClient.newBuilder().sslSocketFactory(null);
                     *
                     * //httpClient.setHostnameVerifier(hostnameVerifier);
                     * httpClient.newBuilder().hostnameVerifier(hostnameVerifier);
                     *
                     * //public long contentLength() { //throws IOException {
                     * public long contentLength() //throws IOException {
                     * 
                     * //public BufferedSource source() throws IOException {
                     * public BufferedSource source() { //throws IOException {
                     *
                     */


                    // upgrade to okhttp3
                    if (null != line)
                    {
                        #region Imports

                        if (line.Contains(TOKEN_OLD_CALL))
                        {
                            replaceLine = TOKEN_CALL + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_CALLBACK))
                        {
                            replaceLine = TOKEN_CALLBACK + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_CREDENTIALS))
                        {
                            replaceLine = TOKEN_CREDENTIALS + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_FORM_ENCODING_BUILDER))
                        {
                            replaceLine = TOKEN_FORM_ENCODING_BUILDER + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_HEADERS))
                        {
                            replaceLine = TOKEN_HEADERS + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_HTTP_CLIENT))
                        {
                            replaceLine = TOKEN_HTTP_CLIENT + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_HTTP_LOGGING))
                        {
                            replaceLine = TOKEN_HTTP_LOGGING + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_HTTP_LOGGING_LEVEL))
                        {
                            replaceLine = TOKEN_HTTP_LOGGING_LEVEL + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_HTTP_METHOD))
                        {
                            replaceLine = TOKEN_HTTP_METHOD + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_MEDIA_TYPE))
                        {
                            replaceLine = TOKEN_MEDIA_TYPE + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_MULTIPART_BUILDER))
                        {
                            replaceLine = TOKEN_MULTIPART_BUILDER + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_REQUEST))
                        {
                            replaceLine = TOKEN_REQUEST + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_REQUEST_BODY))
                        {
                            replaceLine = TOKEN_REQUEST_BODY + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_RESPONSE))
                        {
                            replaceLine = TOKEN_RESPONSE + Environment.NewLine;
                        }
                        else if (line.Contains(TOKEN_OLD_RESPONSE_BODY))
                        {
                            replaceLine = TOKEN_RESPONSE_BODY + Environment.NewLine;
                        }

                        #endregion

                        if (null == replaceLine && (
                            line.Contains(TOKEN_OLD_NAMESPACE_CALL) ||
                            line.Contains(TOKEN_OLD_NAMESPACE_INTERCEPTOR) ||
                            line.Contains(TOKEN_OLD_NAMESPACE_RESPONSE) ||
                            line.Contains(TOKEN_OLD_GET_CONNECT_TIMEOUT) ||
                            line.Contains(TOKEN_OLD_TYPE_FORM_ENCODING_BUILDER) ||
                            line.Contains(TOKEN_OLD_TYPE_MULTIPART_BUILDER) ||
                            line.Contains(TOKEN_OK1_MULTIPART) ||
                            line.Contains(TOKEN_OLD_FAILURE_REQUEST) ||
                            line.Contains(TOKEN_OLD_ASYNC_RESPONSE) ||
                            line.Contains(TOKEN_OLD_SET_CONNECT_TIMEOUT) ||
                            line.Contains(TOKEN_OLD_SET_SSL) ||
                            line.Contains(TOKEN_OLD_SET_HOSTNAME_VERIFIER) ||
                            line.Contains(TOKEN_OLD_CONTENT_LENGTH) ||
                            line.Contains(TOKEN_OLD_BUFFERED_SOURCE)))
                        {
                            replaceLine =
                                line.Replace(TOKEN_OLD_NAMESPACE_CALL, TOKEN_NAMESPACE_CALL).
                                Replace(TOKEN_OLD_NAMESPACE_INTERCEPTOR, TOKEN_NAMESPACE_INTERCEPTOR).
                                Replace(TOKEN_OLD_NAMESPACE_RESPONSE, TOKEN_NAMESPACE_RESPONSE).
                                Replace(TOKEN_OLD_GET_CONNECT_TIMEOUT, TOKEN_GET_CONNECT_TIMEOUT).
                                Replace(TOKEN_OLD_TYPE_FORM_ENCODING_BUILDER, TOKEN_TYPE_FORM_ENCODING_BUILDER).
                                Replace(TOKEN_OK1_MULTIPART, TOKEN_OK3_MULTIPART). //put before TOKEN_OLD_TYPE_MULTIPART_BUILDER
                                Replace(TOKEN_OLD_TYPE_MULTIPART_BUILDER, TOKEN_TYPE_MULTIPART_BUILDER).
                                Replace(TOKEN_OLD_FAILURE_REQUEST, TOKEN_FAILURE_REQUEST).
                                Replace(TOKEN_OLD_ASYNC_RESPONSE, TOKEN_ASYNC_RESPONSE).
                                Replace(TOKEN_OLD_SET_CONNECT_TIMEOUT, TOKEN_SET_CONNECT_TIMEOUT).
                                Replace(TOKEN_OLD_SET_SSL, TOKEN_SET_SSL).
                                Replace(TOKEN_OLD_SET_HOSTNAME_VERIFIER, TOKEN_SET_HOSTNAME_VERIFIER);
                            if (fileInfo.Name.Equals("ProgressResponseBody.java"))
                            {
                                if (null == replaceLine)
                                {
                                    replaceLine = line.Replace(TOKEN_OLD_CONTENT_LENGTH, TOKEN_CONTENT_LENGTH).
                                        Replace(TOKEN_OLD_BUFFERED_SOURCE, TOKEN_BUFFERED_SOURCE);
                                }
                                else
                                {
                                    replaceLine = replaceLine.Replace(TOKEN_OLD_CONTENT_LENGTH, TOKEN_CONTENT_LENGTH).
                                        Replace(TOKEN_OLD_BUFFERED_SOURCE, TOKEN_BUFFERED_SOURCE);
                                }
                            }
                            replaceLine = replaceLine + Environment.NewLine;
                        }

                    }

                    if (null != replaceLine)
                    {
                        newContent += replaceLine;
                    }
                    else if (null != line)
                    {
                        newContent += line;
                        newContent += Environment.NewLine;
                    }
                }
                while (line != null);
            }

            if (originalContent != null &&
                newContent != null &&
                !originalContent.Equals(newContent))
            {
                const bool debug = false;

                if (debug)
                {
                    string tempFolderA = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "a";
                    if (!Directory.Exists(tempFolderA))
                    {
                        Directory.CreateDirectory(tempFolderA);
                    }
                    string tempFileA = tempFolderA + Path.DirectorySeparatorChar + fileInfo.Name;
                    using (StreamWriter sw = new StreamWriter(tempFileA))
                    {
                        sw.Write(originalContent);
                        sw.Flush();
                    }

                    string tempFolderB = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "b";
                    if (!Directory.Exists(tempFolderB))
                    {
                        Directory.CreateDirectory(tempFolderB);
                    }
                    string tempFileB = tempFolderB + Path.DirectorySeparatorChar + fileInfo.Name;
                    using (StreamWriter sw = new StreamWriter(tempFileB))
                    {
                        sw.Write(newContent);
                        sw.Flush();
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(fileInfo.FullName))
                    {
                        sw.Write(newContent);
                        sw.Flush();
                    }
                }
            }

            SetStatus(string.Format("Status: Processed: {0}", fileInfo.Name));
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            if (!CheckIsReady())
            {
                return;
            }

            CacheFolder();

            ThreadStart threadStart = new ThreadStart(() =>
            {
                DirectoryInfo root = new DirectoryInfo(txtFolder.Text);
                if (root.Exists)
                {
                    foreach (DirectoryInfo child in root.GetDirectories())
                    {
                        if (child.Name == "src")
                        {
                            FindAllChildren(child, "java", ProcessFile);
                        }
                    }
                }
                SetStatus("Status: DONE");
            });
            Thread thread = new Thread(threadStart);
            thread.Start();
        }

        private void txtFolder_TextChanged(object sender, EventArgs e)
        {
            CheckIsReady();
        }
    }
}
