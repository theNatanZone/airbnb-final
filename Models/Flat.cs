
using AirbnbProj2.DAL;
using System;
using System.Data.SqlClient;
using System.Diagnostics;

namespace AirbnbProj2.Models
{
    public class Flat
    {
        private int id;
        private string? city;
        private string? address;
        private int numberOfRooms;
        private double price;

        private static readonly List<Flat> flatsList = new List<Flat>();

        public Flat() { }
        public Flat(int id, string? city, string? address, int numberOfRooms, double price)
        {
            Id = id;
            City = city;
            Address = address;
            NumberOfRooms = numberOfRooms;
            Price = price;
        }

        public static Flat CreateFlatFromDb(int _id, string? _city, string? _address, int _numberOfRooms, double _price) 
        {
            var flat = new Flat
            {
                Id = _id,
                City = _city,
                Address = _address,
                NumberOfRooms = _numberOfRooms,
                price = _price // prevent double discount calculation for existing flats
            };
            return flat;
        }

        public int Id { get => id; internal set => id = value; }
        public string? City { get => city; set => city = value; }
        public string? Address { get => address; set => address = value; }
        public int NumberOfRooms { get => numberOfRooms; set => numberOfRooms = value; }
        public double Price
        {
            get => price;
            set
            {
                if (value > 100 && NumberOfRooms > 1)
                    price = Discount(value);
                else
                    price = value;
            }
        }

        public static double Discount(double price)
        {
            return price * 0.9;
        }

    }
}
