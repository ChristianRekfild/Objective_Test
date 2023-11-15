using ApartmentData.CustomExceprion;
using ApartmentData.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Reflection.PortableExecutable;

namespace ApartmentData.Helper
{
    public class SQLHelper
    {
        private readonly string _connectionString = "Host=localhost;Port=5432;Database=ApartmentData;Username=Chris;Password=2113";

        /// <summary>
        /// Возвращает лист аппартаментов по количеству комнат.
        /// </summary>
        /// <param name="requiredNumOfRooms">Количество комнат. Допускается Null</param>
        /// <returns>List доступных аппартаментов</returns>
        /// <exception cref="IncorrectRequestForApartment">Низя указывать отрицательное кол-во комнат!</exception>
        public async Task<List<ApartmentDTO_withPriceOnly>> GetApartmentsByRoomNumber(int? requiredNumOfRooms)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                string errMessage = "Невозможно найти квартиру с отрицательным числом комнат. Попробуйте данный запрос во вселенной с Неевклидовой геометрией.";

                List<ApartmentDTO_withPriceOnly> apartmentDtoList = new List<ApartmentDTO_withPriceOnly>();

                conn.Open();
                NpgsqlCommand command;

                if (requiredNumOfRooms is null)
                {
                    string strCommand = "select app.\"Id\", app.\"NumberOfRooms\", app.\"Url\", " +
                            "( " +
                                "select  pr.\"Price\" " +
                                "from public.\"ApartmentCost\" as pr " +
                                "where pr.\"ApartmentId\" = app.\"Id\" " +
                                "limit 1 " +
                            ") as Price " +
                        "from public.\"Apartments\" AS app ";

                    command = new NpgsqlCommand(strCommand, conn); //("SELECT * FROM public.\"Apartments\"", conn);
                }

                    
                else if (requiredNumOfRooms < 0)
                    throw new IncorrectRequestForApartment(errMessage);
                else
                {
                    string strCommand = "select app.\"Id\", app.\"NumberOfRooms\", app.\"Url\", " +
                            "( " +
                                "select  pr.\"Price\" " +
                                "from public.\"ApartmentCost\" as pr " +
                                "where pr.\"ApartmentId\" = app.\"Id\" " +
                                "limit 1 " +
                            ") as Price " +
                        "from public.\"Apartments\" AS app " +
                        "where app.\"NumberOfRooms\" = @paramnumofrooms";

                    command = new NpgsqlCommand(strCommand, conn);


                    // command = new NpgsqlCommand("SELECT * FROM public.\"Apartments\" AS app WHERE app.numberofrooms = @paramnumofrooms", conn);
                    command.Parameters.AddWithValue("paramnumofrooms", requiredNumOfRooms);
                }
                    

                // Execute the query and obtain the value of the first column of the first row
                var items = await command.ExecuteReaderAsync();

                while (items.Read())
                {
                    int id = (int) items.GetValue(0);
                    int numberOfRooms = (int)items.GetValue(1);
                    string url = items.GetValue(2).ToString();
                    decimal price;
                    try
                    {
                        price = (decimal) items.GetValue(3);
                    } catch (Exception ex)
                    {
                        // Если мы тут, то значит в таблице цен не было сопоставления
                        price = 0;
                    }
                    

                    var apartmentDto_PriceOnly = new ApartmentDTO_withPriceOnly()
                    {
                        Id = id,
                        NumberOfRooms = numberOfRooms,
                        Url = url,
                        Price = price
                    };

                    apartmentDtoList.Add(apartmentDto_PriceOnly);
                }

                return apartmentDtoList;
            }
        }

        public async Task<ApartmentDTO_withPriceList> GetDetailInfoFromApartment(int id)
        {
            ApartmentDTO_withPriceList apartment = null;
            List<ApartmentCostDTO> ApartmentPrices = new List<ApartmentCostDTO>();

            using (NpgsqlConnection conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                string strCommand = "select app.\"Id\", app.\"NumberOfRooms\", app.\"Url\" " +
                    "from public.\"Apartments\" AS app " +
                    "where app.\"Id\" = @paramId";

                NpgsqlCommand command = new NpgsqlCommand(strCommand, conn);
                command.Parameters.AddWithValue("paramId", id);

                //var items = await command.ExecuteReaderAsync();

                bool dataReceived = false; // были ли получены данные

                using (var items = await command.ExecuteReaderAsync())
                {
                    while (items.Read())
                    {
                        dataReceived = true;

                        if (items.GetValue(0) is null
                            || items.GetValue(1) is null
                            || items.GetValue(2) is null
                            ) throw new ApartmentHaveBadData("Упс! Что-то пошло не так...");

                        int _id = (int)items.GetValue(0);
                        int _numberOfRooms = (int)items.GetValue(1);
                        string _url = items.GetValue(2).ToString();

                        apartment = new ApartmentDTO_withPriceList()
                        {
                            Id = _id,
                            NumberOfRooms = _numberOfRooms,
                            Url = _url,
                        };

                        int a = 0;
                    }
                }
                if (!dataReceived) throw new ApartmentNotFound("Не найдено апартаментов с указанным Id");

                // получаем данные с таблицы стоимости
                string strCommandtoGetPrices = "select appCost.\"Id\", appCost.\"Price\", appCost.\"DateAdded\", appCost.\"ApartmentId\" " +
                    "from public.\"ApartmentCost\" AS appCost " +
                    "where appCost.\"ApartmentId\" = @paramid ";

                NpgsqlCommand commandToGetPrices = new NpgsqlCommand(strCommandtoGetPrices, conn);
                commandToGetPrices.Parameters.AddWithValue("paramid", id);

                //var prices = await commandToGetPrices.ExecuteReaderAsync();

                using (var prices = await commandToGetPrices.ExecuteReaderAsync())
                {
                    while (prices.Read())
                    {
                        if (prices.GetValue(0) is null
                            || prices.GetValue(1) is null
                            || prices.GetValue(2) is null
                            || prices.GetValue(3) is null
                            ) throw new ApartmentHaveBadData("Упс! Что-то пошло не так...");

                        int _id = (int)prices.GetValue(0);
                        decimal _price = (decimal)prices.GetValue(1);
                        DateTime _dateAdded = (DateTime)prices.GetValue(2);
                        int _apartmentId = (int)prices.GetValue(3);

                        ApartmentPrices.Add(
                            new ApartmentCostDTO()
                            {
                                Id = _id,
                                Price = _price,
                                DateAdded = _dateAdded,
                                ApartmentId = _apartmentId
                            });
                    }
                }
        
            }

            apartment.ApartmentPrices = ApartmentPrices;

            return apartment;
        }


    }
}
