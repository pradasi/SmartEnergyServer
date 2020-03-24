#to load the model
from tensorflow.keras.models import load_model
#to read the data in csv
from pandas import read_csv
#to convert labeled to encoded value
from sklearn.preprocessing import LabelEncoder
#data preprocessing by normalizing
from sklearn.preprocessing import MinMaxScaler
#to join two list
from numpy import concatenate
#to concat two numpy array
from numpy import column_stack
#for exception
import sys
try:
	#reading the data
	weatherData = read_csv('PythonFiles\weatherForecast.csv', header=0, index_col=0)

	#separarting the data
	featuresUsedToPredict = weatherData.values[1:,:-1]
	prevWindData = weatherData.values[:-1,-1]

	#encoding wind direction
	encoder = LabelEncoder()
	featuresUsedToPredict[:, 2] = encoder.fit_transform(featuresUsedToPredict[:,2])

	#ensure all data is float
	featuresUsedToPredict = featuresUsedToPredict.astype('float32')

	#creating object of scaler
	scaler = MinMaxScaler(feature_range=(0, 1))

	#combining the data according to our model
	featuresUsedToPredict = column_stack((featuresUsedToPredict,prevWindData))

	#load the model
	model = load_model('PythonFiles\my_wind_model')

	#scaling the input data
	scaled = scaler.fit_transform(featuresUsedToPredict)

	#shaping to predict
	values = scaled.reshape(scaled.shape[0], 1 , scaled.shape[1])

	#predicting
	yhat = model.predict(values)

	#converting back to normal shape
	values = values.reshape((values.shape[0], values.shape[2]))

	#inversing the scaling
	inv_yhat = concatenate((values[:, :-1], yhat), axis=1)
	inv_yhat = scaler.inverse_transform(inv_yhat)
	inv_yhat = inv_yhat[:,-1]

	#extracting the index dates for json
	indexDate = weatherData.index[1:].to_list()

	#initilizing dictionary
	preditctedWindData = {}

	#enumerating and appending
	for i,date in enumerate(indexDate):
		dateNew = date.split(' ')[1]
		preditctedWindData[dateNew] = inv_yhat[i]

	import json

	myJson = json.dumps(preditctedWindData)
	print(myJson)

except:
	print("Oops!",sys.exc_info(),"occured.")
