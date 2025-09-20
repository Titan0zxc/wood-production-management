using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Система_для_предприятия.Models
{

    public class CatalogItem
    {
        public string Type;
        public string ModelName;
        public string Dimensions;
        public string Material;
        public decimal Price;

        public override string ToString()
        {
            return $"{ModelName} ({Dimensions}, {Material}) — {Price} руб.";
        }

        public static CatalogItem FromString(string line)
        {
            var parts = line.Split(';');

            if (parts.Length != 5)
                throw new FormatException($"Ошибка разбора строки: \"{line}\". Ожидалось 5 полей, получено {parts.Length}.");

            if (!decimal.TryParse(parts[4], out decimal parsedPrice))
                throw new FormatException($"Неверный формат цены в строке: \"{line}\"");

            return new CatalogItem
            {
                Type = parts[0],
                ModelName = parts[1],
                Dimensions = parts[2],
                Material = parts[3],
                Price = parsedPrice
            };
        }
    }
}
