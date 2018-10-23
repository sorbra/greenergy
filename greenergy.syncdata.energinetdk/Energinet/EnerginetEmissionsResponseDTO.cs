using System.Collections.Generic;

// {
//   "fields": [{"type":"timestamptz","id":"Minutes5UTC"},{"type":"timestamp","id":"Minutes5DK"},{"type":"text","id":"PriceArea"},{"type":"int4","id":"CO2Emission"}],
//   "records": [
//     ["2018-10-21T13:15Z","2018-10-21T15:15","DK2",87],
//     ["2018-10-21T13:15Z","2018-10-21T15:15","DK1",92]
// ]}



namespace Greenergy.Energinet
{
    public class EnerginetEmissionsResponseDTO
    {
        public List<Field> fields { get; set; }
        public List<List<string>> records { get; set; }
    }

    public class Field
    {
        public string type { get; set; }
        public string id { get; set; }
    }
}

