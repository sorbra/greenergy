
namespace Greenergy.Energinet
{
    public class EnerginetBlobCreateResponseDTO
    {
        // The URL of some sort of help page. Useful?
        public string help { get; set; }
        // Whether the request was a success or not
        public bool success { get; set; }
        // The URL of a json file with the actual emissions data requested
        public string result { get; set; }
    }
}