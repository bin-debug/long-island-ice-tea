using EasyVerifyModels.Bureauhouse.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVerifyModels.Bureauhouse
{
    public enum QueryType
    {
        People = 0,
        Address = 1,
        AddressBest = 2,
        IncomePrediction = 3,
        Idv = 4,
        EmailAddress = 5,
        InitiateBiometric = 6,
        CheckBiometric = 7
    }

    public static class UrlQuery
    {
        public static string GetUrl(QueryType url) 
        {
            switch (url)
            {
                case QueryType.People:  return "people/list";
                case QueryType.Address: return "address/list";
                case QueryType.AddressBest: return "address/best";
                case QueryType.IncomePrediction: return "incomeprediction/list";
                case QueryType.Idv: return "idv/list";
                case QueryType.EmailAddress: return "emailaddress/list";
                case QueryType.InitiateBiometric: return "biometricface/initiate";
                case QueryType.CheckBiometric: return "biometricface/check_result";
                default: return "people/list";
            }
        }
    }

    public static class ParameterHelper
    {
        public static Dictionary<string, string> GetParameters(QueryType queryType, string token, string permissiblePurpose, PersonInfoRequest request)
        {
            if (queryType == QueryType.InitiateBiometric)
            {
                return new Dictionary<string, string>
            {
                { "Token", token },
                { "PermissiblePurpose", permissiblePurpose },
                { "IDNumber", request.IDNumber },
                { "CellNumber", request.CellNumber }
            };
            }
            else if (queryType == QueryType.CheckBiometric)
            {
                return new Dictionary<string, string>
            {
                { "Token", token },
                { "PermissiblePurpose", permissiblePurpose },
                { "Pincode", request.Pincode },
                { "IDNumber", request.IDNumber }
            };
            }
            else
            {
                return new Dictionary<string, string>
            {
                { "Token", token },
                { "PermissiblePurpose", permissiblePurpose },
                { "IDNumber", request.IDNumber },
            };
            }
        }
    }
}
