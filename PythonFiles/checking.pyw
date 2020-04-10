from pandas import read_csv

data = read_csv('weatherForecast.csv', header=0, index_col=0)
indexDate = data.index[1:].to_list()

preditctedWindData = {}

#enumerating and appending
for i,date in enumerate(indexDate):
	dateNew = date.split(' ')[1].split(':')[0]
	print(dateNew)