# AutoGlobalUsing

Questo piccolo progetto automatizza il processo di riorganizzazione degli `using` all'interno dei progetti C# (versioni 10 e superiori).

L'idea è semplice: anziché avere gli `using` sparsi in diversi file sorgente, lo strumento li raccoglie in un unico file, migliorando la leggibilità e mantenibilità del codice.

Lo strumento può essere eseguito **su un singolo progetto C# o su un'intera cartella di progetti in modo ricorsivo**. Per ogni progetto rilevato, viene generato un file di `GlobalUsing` separato, spostando tutti gli using nel nuovo file e mantenendo la struttura pulita.

### Funzionalità
- **Supporto per singolo progetto o cartella di progetti**: puoi analizzare un singolo progetto o eseguire una scansione ricorsiva di una cartella per elaborare tutti i progetti C# al suo interno.
- **Personalizzazione del nome del file prodotto**: puoi specificare nel file di *appsettings.json** il nome del file finale da produrre.
- **Rimozione automatica di file di `GlobalUsing` esistenti**: Puoi usare un'espressione regolare, sempre configurabile in *appsettings.json* per trovare ed eliminare tutti i file di `GlobalUsing` già presenti nel progetto, garantendo pulizia completa prima della generazione del nuovo file, conservando tutti i singoli global using già presenti in questi file.
- **Gestione degli `using` using**: Puoi decidere se mantenere o rimuovere gli `alias using` durante la riorganizzazione, in base alle tue esigenze.

### Utilizzo

Nelle release, qui affianco, puoi trovare il programma già compilato e scaricabile in un file .zip da decomprimere.
Puoi scegliere se avviarlo direttamente, e inserire il percorso della cartella da console seguendo banalmente le istruzioni, oppure puoi avviarlo passando il nome della cartella da analizzare come argomento.

```
AutoGlobalUsing.exe MY_PATH true
```

Il secondo parametro che vedi a true è ```verbose``` e se posto a true (sarà true di default, comunque) ti mostrerà a schermo tutti gli using che vengono spostati nel nuovo file, per tutti i progetti.

Se il progetto ti è stato utile puoi offrirmi un caffè!

<a href="https://www.buymeacoffee.com/Davidencho" target="_blank"><img src="https://cdn.buymeacoffee.com/buttons/default-orange.png" alt="Buy Me A Coffee" height="41" width="174"></a>