using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using CoinConvert.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            SelectedDate = DateTime.Today;
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
                CurrencyList = GetCurrencyList(DateTime.Today.AddDays(-3));
                OnPropertyChanged(nameof(SelectedDate));
            }
        }

        private string _firstCoin;

        public string FirstCoin
        {
            get => _firstCoin;
            set
            {
                _firstCoin = value;
                OnPropertyChanged(nameof(FirstCoin));
            }
        }

        private string _secondCoin;

        public string SecondCoin
        {
            get => _secondCoin;
            set
            {
                _secondCoin = value;
                OnPropertyChanged(nameof(SecondCoin));
            }
        }

        public List<Currency> GetCurrencyList(DateTime date)
        {
            var reqString = $"https://www.cbr-xml-daily.ru/archive/{date.ToString("yyyy/MM/dd").Replace('.','/')}/daily_json.js";
            var result = httpClient
                .GetStringAsync(reqString).Result;
            var allValutesString = JObject.Parse(result)["Valute"]?.ToString();
            var allValutesDictionary = JsonConvert.DeserializeObject<Dictionary<string, Currency>>(allValutesString);
            return allValutesDictionary?.Select(x => x.Value).ToList();
        }
    }
}