import datetime as dt
import requests
import json

urlForWeather =  "https://api.weather.com/v3/wx/forecast/hourly/15day?apiKey=6532d6454b8aa370768e63d6ba5a832e&geocode=12.97%2C77.598&units=e&language=en-US&format=json"

try:
    request = requests.get(url = urlForWeather)
    data = request.json()
    weatherDataForWind = {}
    
    dateTick = data['validTimeLocal'][:24]
    day = data['dayOfWeek'][:24]
    temperature = data['temperature'][:24]
    pressure = data['pressureMeanSeaLevel'][:24]
    windSpeed = data['windSpeed'][:24]
    weatherCondition = data['wxPhraseLong'][:24]
    
    startDay = day[0]
    count = 1;
    while True:
        if day[count] == startDay:
            count+=1
        else:
            break
        
    weatherDataForDay = {}
    weatherDataForDay['date'] = dateTick[0].split('T')[0]
    weatherDataForDay['day'] = startDay 
    tempWeatherForDay = []
	
    iterable = count//3 + 1
    now = dt.datetime.now()
    date_list = [now + dt.timedelta(minutes=60*x) for x in range(count)]
    indexDate = [x.strftime("%H:%M") for x in date_list]
         
    for i in range(iterable):
        temp = {}
        i = i * 3
        if i > count:
            break
        temp['time'] = indexDate[i]
        temp['temperature'] = str(temperature[i]) + " F"
        temp['pressure'] = str(pressure[i]) + " in"
        temp['windSpeed'] = str(windSpeed[i]) + " mph"
        temp['weatherCondition'] = weatherCondition[i]
        tempWeatherForDay.append(temp)

    weatherDataForDay['data'] = tempWeatherForDay
    myJson = json.dumps(weatherDataForDay)
    print(myJson)

except Exception as ex:
	print("Failed, the exception is {}".format(ex))
	