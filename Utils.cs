using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Xsl;
using System.Xml;
using System.IO;

namespace ASWBXML
{
    /// <summary>
    /// Static class for utility methods
    /// </summary>
    static class Utils
    {
        /// <summary>
        /// Quoted-Printable decoding
        /// RFC2045 - 6.6 Canonical Encoding Model
        /// http://tools.ietf.org/html/rfc2045
        ///
        /// This is often how iOS sends ICS attachments
        /// </summary>
        /// <param name="qpString">Input quoted-printable string</param>
        /// <returns>Decoded string</returns>
        public static string DecodeQP(string qpString)
        {
            // For copying items out, go ahead and change the content encoding
            qpString = qpString.Replace("Content-Transfer-Encoding: quoted-printable", "Content-Transfer-Encoding: 7bit");

            // Soft line break / 78-character line-wrap
            qpString = qpString.Replace("=<br/>", "");
            qpString = qpString.Replace("=0D=0A=20", "");

            // Newline to <br/>
            qpString = qpString.Replace("=0D=0A", "<br/>");

            // Tab to spaces
            qpString = qpString.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");

            // Space to space
            qpString = qpString.Replace("=20", " ");

            // = to =
            qpString = qpString.Replace("=3D", "=");

            return qpString;
        }

        public static string DecodeEmailData(string dataString)
        {
            dataString = System.Web.HttpUtility.HtmlEncode(dataString);
            dataString = dataString.Replace("\r\n ", "<br/>&nbsp;");
            dataString = dataString.Replace("\r\n", "<br/>");
            dataString = dataString.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
            dataString = dataString.Replace("    ", "&nbsp;&nbsp;&nbsp;&nbsp;");

            // For now, only decode ICS items, since I have no example data for other QP data
            if (dataString.ToLower().Contains("content-transfer-encoding: quoted-printable") && dataString.ToLower().Contains(@"text/calendar"))
            {
                dataString = DecodeQP(dataString);
            }

            return dataString;
        }

        /// <summary>
        /// Returns the printable bytes as a string
        /// </summary>
        /// <param name="bytesFromFiddler">byte[] to convert</param>
        /// <returns>string representation of the passed bytes</returns>
        public static string GetByteString(byte[] bytesFromFiddler)
        {
            StringBuilder sb = new StringBuilder();

            // Output the byte pairs
            foreach (byte singleByte in bytesFromFiddler)
            {
                sb.Append(singleByte.ToString("x2").ToUpper());
            }

            return sb.ToString();
        }

        public static string GetXmlString(XmlDocument xmlDoc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter xmlw = new XmlTextWriter(sw);
            xmlw.Formatting = Formatting.Indented;
            xmlDoc.WriteTo(xmlw);
            xmlw.Flush();

            return sw.ToString();
        }
    }
}
