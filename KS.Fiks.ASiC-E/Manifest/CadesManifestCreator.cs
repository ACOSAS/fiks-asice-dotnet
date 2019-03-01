using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using KS.Fiks.ASiC_E.Model;
using KS.Fiks.ASiC_E.Xsd;

namespace KS.Fiks.ASiC_E.Manifest
{
    public class CadesManifestCreator : AbstractManifestCreator
    {
        public const string FILENAME = "META-INF/asicmanifest.xml";

        public override ManifestContainer CreateManifest(IEnumerable<AsicPackageEntry> entries)
        {

            var manifest = new ASiCManifestType
            {
                DataObjectReference = entries.Select(ToDataObject).ToArray()
            };
            using (var outStream = new MemoryStream())
            {
                new XmlSerializer(typeof(ASiCManifestType)).Serialize(outStream, manifest);
                return new ManifestContainer(FILENAME, outStream.ToArray(), CreateSignatureRef());
            }
        }

        private static DataObjectReferenceType ToDataObject(AsicPackageEntry packageEntry)
        {
            if (packageEntry == null)
            {
                return null;
            }

            return new DataObjectReferenceType
            {
                MimeType = packageEntry.Type.ToString(),
                DigestMethod = new DigestMethodType
                {
                    Algorithm = packageEntry.MessageDigestAlgorithm.Uri.ToString()
                },
                DigestValue = packageEntry.Digest.GetDigest(),
                URI = packageEntry.FileName
            };
        }
    }
}