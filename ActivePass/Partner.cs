using System;
using Newtonsoft.Json;

namespace ActivePass
{
    public class Partner : IEquatable<Partner>
    {
        public Partner(string company, string website, [JsonProperty("ap_uri")] string partnerId, [JsonProperty("image_url")] string imageUrl, string address, string district, string city, string zip, string latitude, string longitude)
        {
            Company = company;
            Website = website;
            PartnerId = partnerId;
            ImageUrl = imageUrl;
            Address = address;
            District = district;
            City = city;
            Zip = zip;
            Latitude = latitude;
            Longitude = longitude;
        }

        public string Company { get; }
        public string Website { get; }
        public string PartnerId { get; }
        public string ImageUrl { get; }
        public string Address { get; }
        public string District { get; }
        public string City { get; }
        public string Zip { get; }
        public string Latitude { get; }
        public string Longitude { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Partner);
        }

        public bool Equals(Partner? other)
        {
            return other != null &&
                   Company == other.Company &&
                   Website == other.Website &&
                   PartnerId == other.PartnerId &&
                   ImageUrl == other.ImageUrl &&
                   Address == other.Address &&
                   District == other.District &&
                   City == other.City &&
                   Zip == other.Zip &&
                   Latitude == other.Latitude &&
                   Longitude == other.Longitude;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Company);
            hash.Add(Website);
            hash.Add(PartnerId);
            hash.Add(ImageUrl);
            hash.Add(Address);
            hash.Add(District);
            hash.Add(City);
            hash.Add(Zip);
            hash.Add(Latitude);
            hash.Add(Longitude);
            return hash.ToHashCode();
        }

        public override string ToString()
        {
            return $"Company: {Company}\nWebsite: {Website}\nPartnerId: {PartnerId}\nImageUrl: '{ImageUrl}'\nAddress: {Address}\nDistrict: {District}\nCity: {City}\nZip: {Zip}\nLatitude: {Latitude}, Longitude: {Longitude}";
        }
    }
}
