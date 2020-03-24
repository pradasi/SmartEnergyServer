from datetime import datetime
from pandas import DataFrame
import requests
import json

url = "https://api.weather.com/v3/wx/observations/current?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json";
request = requests.get(url=url)
data = request.json()

weatherData = {}

weatherData["cloudPhrase"] = data['cloudCoverPhrase']
weatherData["dayOfWeek"] = data['dayOfWeek']
weatherData["dayOrNight"] = data['dayOrNight']
weatherData["visibility"] = data['visibility']
weatherData["pressure"] = data['pressureAltimeter']
weatherData["relativeHumidity"] = data['relativeHumidity']
weatherData["temperature"] = data['temperature']
weatherData["dewPoint"] = data['temperatureDewPoint']
weatherData["windDirection"] = data['windDirectionCardinal']
weatherData["windSpeed"] = data['windSpeed']
dateTime = datetime.strptime(data['validTimeLocal'],("%Y-%m-%dT%H:%M:%S+0530"))
weatherData['time'] = dateTime.strftime("%H:%M:%M")
weatherData['date'] = dateTime.strftime("%Y-%m-%d")
myJson = json.dumps(weatherData)
print(myJson)