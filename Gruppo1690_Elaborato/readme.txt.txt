All'interno della cartella compressa sono presenti due cartelle: una contiene il progetto di visual studio completo 
e l'altra l'eseguibile. Per provare l'applicativo è sufficiente aprire la cartella WaveRock-Applicativo e avviare 
l'eseguibile 'WaveRock' presente al suo interno. Non sono presenti credenziali.
 
NOTA:
Il file del database si trova all’interno della cartella WaveRock-Progetto in WaveRock/WaveRock.
E' poi presente una copia nella cartella WaveRock-Applicativo. 
Tramite l'esecuzione di visual studio la ricerca avviene solitamente in bin/debug, dove però è presente solo una 
copia dell’originale che viene rigenerata ad ogni esecuzione. Nella classe MainWindow del progetto sono presenti 
delle righe di codice per modificare la cartella in cui visual studio ricerca il DB (tramite indirizzo relativo).
Attualmente la ricerca avviene in WaveRock/WaveRock, dove è presente l’originale, e non una sua copia; 
dunque le modifiche sono permanenti (vedi inserimenti, ecc..).
Progetto ed esegubile lavorano quindi su copie diverse, ma entrambe copie in cui le modifiche sono permanenti.
Per eventuali modifiche bisogna tenere a mente questo dettaglio, per esempio se si vuole replicare l'eseguibile o
spostare alcuni file e/o cartelle.

 




 
