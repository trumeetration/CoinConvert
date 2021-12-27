using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using CoinConvert.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CoinConvert.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private HttpClient httpClient;

        public MainViewModel()
        {
            httpClient = new HttpClient();
            CurrencyList = new List<Currency>();
            SelectedDate = DateTime.Parse(Preferences.Get("Date", DateTime.Now.ToString()));
            FromValue = Preferences.Get("FromValue", "0");
            if (Preferences.Get("FromCoin", "") == "")
                FirstCoin = CurrencyList[0];
            else
                FirstCoin = CurrencyList.First(x => x.CharCode == Preferences.Get("FromCoin", ""));
            if (Preferences.Get("ToCoin", "") == null)
                SecondCoin = CurrencyList[1];
            else
                SecondCoin = CurrencyList.First(x => x.CharCode == Preferences.Get("ToCoin", ""));
        }

        private List<Currency> _currencyList;

        public List<Currency> CurrencyList
        {
            get => _currencyList;
            set
            {
                _currencyList = value;
                OnPropertyChanged(nameof(CurrencyList));
            }
        }


        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set
            {
                _selectedDate = value;
                CurrencyList = GetCurrencyList();
                Preferences.Set("Date", _selectedDate.ToString());
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private Currency _firstCoin;

        public Currency FirstCoin
        {
            get => _firstCoin;
            set
            {
                _firstCoin = value;
                Preferences.Set("FromCoin", value?.CharCode);
                FromValue = FromValue;
                OnPropertyChanged(nameof(FirstCoin));
            }
        }

        private Currency _secondCoin;

        public Currency SecondCoin
        {
            get => _secondCoin;
            set
            {
                _secondCoin = value;
                Preferences.Set("ToCoin", value?.CharCode);
                FromValue = FromValue;
                OnPropertyChanged(nameof(SecondCoin));
            }
        }

        private string _fromValue;

        public string FromValue
        {
            get => _fromValue;
            set
            {
                _fromValue = value;
                double a;
                if (Double.TryParse(_fromValue, out a) && SecondCoin != null && FirstCoin != null)
                {
                    ToValue = $"{a * (FirstCoin.Value / FirstCoin.Nominal ) / (SecondCoin.Value / SecondCoin.Nominal):0.00}";
                }
                Preferences.Set("FromValue", _fromValue);
                OnPropertyChanged(nameof(FromValue));
            }
        }

        private string _toValue;

        public string ToValue
        {
            get => _toValue;
            set
            {
                _toValue = value;
                OnPropertyChanged(nameof(ToValue));
            }
        }


        public List<Currency> GetCurrencyList()
        {
            try
            {
                var reqString =
                    $"https://www.cbr-xml-daily.ru/archive/{SelectedDate.ToString("yyyy/MM/dd").Replace('.', '/')}/daily_json.js";
                var result = httpClient
                    .GetStringAsync(reqString).Result;
                var allValutesString = JObject.Parse(result)["Valute"]?.ToString();
                var allValutesDictionary = JsonConvert.DeserializeObject<Dictionary<string, Currency>>(allValutesString);
                return allValutesDictionary?.Select(x => x.Value).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SelectedDate = SelectedDate.AddDays(-1);
                return GetCurrencyList();
            }
        }

        public void SaveAllData()
        {
            Preferences.Set("Date", SelectedDate.ToString());
            Preferences.Set("FromCoin", FirstCoin.CharCode);
            Preferences.Set("ToCoin", SecondCoin.CharCode);
            Preferences.Set("FromValue", FromValue);
        }
    }
}