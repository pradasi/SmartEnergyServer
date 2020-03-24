from datetime import datetime
from pandas import DataFrame
import requests
import json
import sys

try:
	url =  "https://api.weather.com/v3/wx/forecast/hourly/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json"
	request = requests.get(url=url)
	data = request.json()

	weatherData = []

	temperature = data['temperature'][:24]
	dewpt = data['temperatureDewPoint'][:24]
	windDir = data['windDirectionCardinal'][:24]
	humidity = data['relativeHumidity'][:24]
	pressure = data['pressureMeanSeaLevel'][:24]
	windSpeed = data['windSpeed'][:24]
	timeStamp = data['validTimeUtc'][:24]
	date = []

	for time in timeStamp:
		date.append(datetime.fromtimestamp(time))

	weatherData.append(date)
	weatherData.append(temperature)
	weatherData.append(dewpt)
	weatherData.append(windDir)
	weatherData.append(humidity)
	weatherData.append(pressure)
	weatherData.append(windSpeed)
	forecastData = DataFrame(weatherData)
	forecastData = forecastData.T
	columns = ['DateTime','Temperature','DewPoint','WindDirection','Humidity','Pressure','WindSpeed']

	forecastData.to_csv('PythonFiles\weatherForecast.csv', header=columns, index=False)
	print("success")
	
except:
	print("Oops!",sys.exc_info()[0],"occured.")