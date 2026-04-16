import { useEffect, useState } from 'react'
import './App.css'

function App() {
    const [forecasts, setForecasts] = useState([])

    useEffect(() => {
        fetch('http://localhost:5003/weatherforecast')
            .then(response => response.json())
            .then(data => setForecasts(data))
            .catch(err => console.error(err))
    }, [])

    return (
        <div className="App">
            <h1>WetherForecast</h1>
            <table border="1" style={{ margin: 'auto' }}>
                <thead>
                    <tr>
                        <th>Date</th>
                        <th>C</th>
                        <th>Desc</th>
                    </tr>
                </thead>
                <tbody>
                    {forecasts.map((f, index) => (
                        <tr key={index}>
                            <td>{f.date}</td>
                            <td>{f.temperatureC}</td>
                            <td>{f.summary}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}

export default App