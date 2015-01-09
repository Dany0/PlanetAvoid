# PlanetAvoid
GDI hra, školní projekt ze střední.

![](http://i.imgur.com/r0Hdwno.png)
**Fig 1-1 Here we can see a mouse pointer.**

Features:
 - běží na "herním enginu" původně dělaném pro vizualizátor grafů...
 - vlastnoručně dělaný, pěkný sidebar s kterým se dá hýbat oknem
 - launcher s nastavením rozlišení, eye candy a debug
 - je v tom skrytá nějaká hra
 - simulace n-body gravitace (haha)
 - rychlý box blur - unsafe
 - všechno vykreslené pomocí GDI - žádné bitmapy. to ne že by "herní engine" sprity nepodporoval, prostě jsem to vzal jako výzvu
 - neomezené levelování
 - spadne na cca stém levelu (UPDATE: cca padesátém levelu)
 - DISCO! každých deset levelů
 - loading screen
 - binární sluneční soustavy!
 - asteroidy z ementálu
 - spousta debug možností
 - spousta různorodých, procedurálně generovaných planet
 - spousta hnusných barev
 - spousta zacyklení a stackoverflow, pramenicích ze špatného objektového návrhu
 
 Věci co jsem nedodělal:
 - menu - vlastně to jako menu začalo, vzal jsem menu z nedokončeného vizualizátoru grafů a udělal z toho "hru", ale menu pro hru jsem nakonec nestihl udělat
 - highscore - see above
 - ukládání configu to XML, doufal jsem že bude fungovat nějaká magická serializace, ale ono ne a ne. obdobně jsem před tím dělal serializaci do JSON ale tady to nefungovalo, už nevím proč je to dávno
 - měla v tom být hra, ale přestalo mně to bavit. *GDI je zlo*
