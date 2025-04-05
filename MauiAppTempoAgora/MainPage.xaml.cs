using MauiAppTempoAgora.Models;
using MauiAppTempoAgora.Services;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Reflection.Metadata;

namespace MauiAppTempoAgora
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_Clicked_Previsao(object sender, EventArgs e)
        {

            
                try
                {
                    if (!string.IsNullOrEmpty(txt_cidade.Text))
                    {
                        Tempo? t = await DataService.GetPrevisao(txt_cidade.Text);

                        if (t != null)
                        
                            {
                            string dados_previsao = "";

                            dados_previsao = $"Latitude: {t.lat} \n" +
                                             $"Longitude: {t.lon} \n" +
                                             $"Nascer do Sol: {t.sunrise} \n" +
                                             $"Por do Sol: {t.sunset} \n" +
                                             $"Temp Máx: {t.temp_max} \n" +
                                             $"Temp Min: {t.temp_min} \n" +
                                             $"Descrição: {t.description} \n" +
                                             $"Velocidade do Vento: {t.speed} \n" +
                                             $"Visibilidade: {t.visibility} \n";

                            lbl_res.Text = dados_previsao;

                        string mapa = $"https://embed.windy.com/embed.html?" +
                        $"type=map&location=coordinates&metricRain=mm&metricTemp=°C" +
                        $"&metricWind=km/h&zoom=5&overlay=wind&product=ecmwf&level=surface" +
                        $"&lat={t.lat.ToString().Replace(",", ".")}&lon={t.lon.ToString().Replace(",", ".")}"; 

                        wv_mapa.Source = mapa;



                    }
                        else
                        {
                            lbl_res.Text = "Cidade não encontrada ou sem dados de previsão.";
                        }
                    }
                    else
                    {
                        lbl_res.Text = "Preencha a cidade.";
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    lbl_res.Text = "Cidade não encontrada.";
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Ops", ex.Message, "OK");
                }
            }

        private async void Button_Clicked_Localizacao(object sender, EventArgs e)
        {
            try
            {
                GeolocationRequest request = new GeolocationRequest(
                    GeolocationAccuracy.Medium,
                    TimeSpan.FromSeconds(10)               
                    );
                Location? local = await Geolocation.Default.GetLocationAsync(request);

                if (local != null)
                {
                    string local_disp = $"Latitude: {local.Latitude} \n" +
                                        $"Longitude: {local.Longitude} \n";

                    lbl_coords.Text = local_disp;

                    GetcCidade(local.Latitude, local.Longitude);


                }else
                {
                    lbl_coords.Text = "Nenhuma Localização";   
                }
                

            }
            catch (FeatureNotSupportedException fnsEx)
            {
                await DisplayAlert("Erro: Dispositivo não Suporta", fnsEx.Message, "OK");
            }
            catch (FeatureNotEnabledException fneex)
            {
                await DisplayAlert("Erro: Localização Desabilitada", fneex.Message, "OK");
            }
            catch (PermissionException pex)
            {
                await DisplayAlert("Erro: Permissão da Localização", pex.Message, "OK");
            }
            catch (Exception ex)
            {
               await DisplayAlert("Erro", ex.Message, "OK");
            }
            }
      private async void GetcCidade(double lat, double lon)
       {
            try
            { 


           IEnumerable<Placemark> places = await Geocoding.Default.GetPlacemarksAsync(lat, lon);
           
           Placemark? place = places.FirstOrDefault();

                if (place != null)
                {
                    txt_cidade.Text = place.Locality;
                }

            }catch (Exception ex)

            {
                await DisplayAlert("Erro: Obtenção do nome da cidade", ex.Message, "OK");
            }

      }


    
    }
}
    