from datetime import datetime
from pandas import DataFrame
import requests
import json
import meteomatics.api as api
#for datetime generation
import datetime as dt


#function to extract value from json
def extractValue(data):
    valueList = []
    for i in range(len(data)):
        valueList.append(data[i]['value'])
    return valueList

#getting current date and preperation to call api
dateTime = dt.datetime.utcnow().strftime("%Y-%m-%dT%H:%M:%S+00:00")
urlForSolar = "https://api.meteomatics.com/"+dateTime+"P2D:PT1H/sun_elevation:d,sun_azimuth:d,t_2m:C,relative_humidity_1000hPa:p,sfc_pressure:hPa,global_rad:W/12.97210,77.59330/json"
urlForWind =  "https://api.weather.com/v3/wx/forecast/hourly/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json"

username = 'cmrit_kumar'
password = 'aM3OVy9uYz7Ei'

try:

	now= dt.datetime.now()
	date_list = [now + dt.timedelta(minutes=60*x) for x in range(0, 25)]
	indexDate=[x.strftime("%H:%M") for x in date_list]
	indexDate = indexDate[1:]
	
	#this is solar
	#calling the api
	dataOfSolar = api.query_api(urlForSolar, username, password )
	#on succesfull call data extraction process
	featuresToPredict = dataOfSolar.json()['data']
    
	toPredict = []
	toPredict.append(indexDate)
	toPredict.append(extractValue(featuresToPredict[0]['coordinates'][0]['dates'][1:25]))    
	toPredict.append(extractValue(featuresToPredict[1]['coordinates'][0]['dates'][1:25]))    
	toPredict.append(extractValue(featuresToPredict[2]['coordinates'][0]['dates'][1:25]))    
	toPredict.append(extractValue(featuresToPredict[3]['coordinates'][0]['dates'][1:25]))   
	toPredict.append(extractValue(featuresToPredict[4]['coordinates'][0]['dates'][1:25]))  
	toPredict.append(extractValue(featuresToPredict[5]['coordinates'][0]['dates'][0:24]))
	##dummy
	solarEnergy = extractValue(featuresToPredict[5]['coordinates'][0]['dates'][1:25])
	toPredict.append(solarEnergy)
	
	now= dt.datetime.now()
	date_list = [now + dt.timedelta(minutes=60*x) for x in range(0, 25)]
	indexDate=[x.strftime("%H:%M") for x in date_list]
	indexDate = indexDate[1:]
	
	columnsSolar = ['DateTime','Elavation','Azimuth','Temperature','Humidity','Pressure','Irradiation','ActualIrradiation']
	toPredictForSolar = DataFrame(toPredict).T
	
	
	#this is wind
	request = requests.get(url = urlForWind)
	data = request.json()

	weatherDataForWind = []

	temperature = data['temperature'][:24]
	dewpt = data['temperatureDewPoint'][:24]
	windDir = data['windDirectionCardinal'][:24]
	humidity = data['relativeHumidity'][:24]
	pressure = data['pressureMeanSeaLevel'][:24]
	windSpeed = data['windSpeed'][:24]
	timeStamp = data['validTimeUtc'][:24]

	weatherDataForWind.append(indexDate)
	weatherDataForWind.append(temperature)
	weatherDataForWind.append(dewpt)
	weatherDataForWind.append(windDir)
	weatherDataForWind.append(humidity)
	weatherDataForWind.append(pressure)
	weatherDataForWind.append(windSpeed)
	forecastData = DataFrame(weatherDataForWind)
	forecastData = forecastData.T
	columns = ['DateTime','Temperature','DewPoint','WindDirection','Humidity','Pressure','WindSpeed']

	forecastData.to_csv('PythonFiles\weatherForecastForWind.csv', header=columns, index=False)
	toPredictForSolar.to_csv('PythonFiles\weatherForecastForSolar.csv', header=columnsSolar, index=False)
	print("success")
	
except Exception as ex:
    print("Failed, the exception is {}".format(ex))