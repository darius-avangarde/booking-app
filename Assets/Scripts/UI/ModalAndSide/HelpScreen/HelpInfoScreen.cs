using System;
using System.Collections;
using System.Collections.Generic;
using UINavigation;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpInfoScreen : MonoBehaviour
{
    [SerializeField]
    private Text infoTitle = null;
    [SerializeField]
    private TextMeshProUGUI infoText = null;
    [SerializeField]
    private RectTransform contentTransform = null;

    public void Initialize(string title)
    {
        switch (title)
        {
            case "Pagina principală":
                infoTitle.text = $"Pagina principală";
                infoText.text = $"    În calendarul din pagina principală aveți toate rezervările făcute de pe proprietatea selectată*, implicit si posibilitatea de a creea o rezervare nouă apăsând pe butonul <sprite name=\"round\" color=#32a3a8><sprite name=\"plus\" color=#FFFFFF> din dreapta, jos, sau apăsând pe o dată din calendar.  {Environment.NewLine}" +
                        $"    *Proprietatea activă se poate schimba din meniul derulant din partea de jos a ecranului.{Environment.NewLine}{Environment.NewLine}" +
                        $"    Puteți viziona notificările* active apăsând pe icoana <sprite name=\"notifications\" tint=1> din colțul din dreapta, sus, a ecranului.{Environment.NewLine}" +
                        $"    *Notificările pot fi activate din pagina de Setări <sprite name=\"settings\" tint=1>.";
                break;
            case "Proprietăți":
                infoTitle.text = "Proprietăți";
                infoText.text = $"    Pentru a crea o proprietate nouă, apăsați pe butonul <sprite name=\"round\" color=#32a3a8><sprite name=\"plus\" color=#FFFFFF> din colțul din dreapta, jos.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În panoul de creare proprietate, va trebui să introduceți numele proprietății și să alegeți dacă proprietatea este de sine stătătoare sau dacă este cu camere.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În cazul în care proprietatea pe care vreți să o creati este cu camere, un meniu suplimentar va apărea în care va trebui să alegeți numărul de etaje al proprietății și numărul de camere per etaj.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În cele din urmă, alegeți tipul de camere (sau de proprietate, in cazul proprietăților fără camere) din meniul derulant. Această opțiune poate fi modificată ulterior prin editarea camerelor în mod individual.";
                break;
            case "Camere":
                infoTitle.text = "Camere";
                infoText.text = $"    Pentru a edita o cameră apăsați pe butonul cu camera dorită din pagina principală, în partea din stânga, sau puteți intra din meniul lateral <sprite name=\"menu\" tint=1>, la secțiunea de proprietăți <sprite name=\"proprietati\" tint=1>, alegeți proprietatea și in cele din urmă camera pe care doriți să o editați.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În panoul de editare caemră, puteți schimba numele camerei, tipul de cameră din meniul derulant și numarul de paturi* din camera respectiva apăsând pe plus <sprite name=\"plus2\" tint=1> sau minus <sprite name=\"minus2\" tint=1>.{Environment.NewLine}{Environment.NewLine}" +
                        $"    *La crearea unei proprietăți cu camere, numarul de paturi din camere este 0 (zero).";
                break;
            case "Clienți":
                infoTitle.text = "Clienți";
                infoText.text = $"    Pentru a adăuga un client nou, apăsați pe butonul <sprite name=\"round\" color=#43ae52><sprite name=\"plus\" color=#FFFFFF> din colțul din dreapta, jos.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În panoul de adăugare clienți, va trebui să introduceți numele, numarul de telefon, e-mail-ul și adresa clientului.{Environment.NewLine}" +
                        $"    *Numele este singurul câmp obligatoriu.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În lista de clienți veți găsi clienți pe care îi aveți salvați. Apăsând pe un client, se va deschide un mic panou cu detaliile clientului respectiv, și 4 butoane de unde veți putea să apelați <sprite name=\"phone\" tint=1> , trimiteți mesaj <sprite name=\"message\" tint=1> , trimiteți e-mail <sprite name=\"email\" tint=1> , sau să editați <sprite name=\"edit\" tint=1> clientul.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În cazul în care vreți să căutați un client, apăsați pe icoana de căutare <sprite name=\"search\" tint=1> din colțul din dreapta, sus.{Environment.NewLine}" +
                        $"    Un client se poate căuta după nume, număr de telefon, e-mail, sau adresă.";
                break;
            case "Rezervări":
                infoTitle.text = "Rezervări";
                infoText.text = $"    Pentru a crea o rezervare nouă, în pagina principală, puteți apăsa pe butonul <sprite name=\"round\" color=#cd7d2e><sprite name=\"plus\" color=#FFFFFF> din colțul din dreapta, jos, sau puteți apăsa pe o dată din calendarul cu camere.{Environment.NewLine}" +
                        $"    În cazul ulterior, data pe care ați apăsat va fi preselectată în meniul de creare rezervări ca dată de început a rezervării.{Environment.NewLine}{Environment.NewLine}" +
                        $"    În panoul de creare rezervări, va trebui să introduceți perioada rezervarii (data de început <sprite name=\"start\" tint=1> și data de sfârșit <sprite name=\"end\" tint=1>), pe care o selectați cu ajutorul calendarului care se va deschide.{Environment.NewLine}{Environment.NewLine}" +
                        $"    Proprietatea și camera o veți putea selecta din cele două meniuri derulante.{Environment.NewLine}" +
                        $"    *La creearea unei rezervări se pot selecta multiple camere din meniul derulant.{Environment.NewLine}{Environment.NewLine}" +
                        $"    Pentru a selecta un client, puteți introduce în câmpul \"Alege clientul\" numele și să-l selectați din meniul derulant care apare*, sau să apăsați pe icoana <sprite name=\"arrow\" tint=1>) din dreapta câmpului și să-l alegeți din lista de clienți care va apărea.{Environment.NewLine}" +
                        $"    *În cazul în care introduceți un nume pe care nu-l aveți în lista de clienți, acesta se va salva automat în momentul în care salvați rezervarea.";
                break;
            case "Filtrare calendar":
                infoTitle.text = "Filtrare calendar";
                infoText.text = $"    Pentru a filtra camerele vizibile în calendarul din pagina principală, apăsați pe icoana de filtre <sprite name=\"filter\" tint=1> din partea stângă a calendarului.{Environment.NewLine}" +
                        $"    În panoul de filtrare aveți opțiunea de a filtra după perioadă, tipul de cameră/proprietate, numărul de paturi și client.{Environment.NewLine}" +
                        $"    Pentru a folosi una din opțiunile de filtrare, va trebui să activați opțiunea prin a apăsa pe caseta de bifat din dreptul fiecărei opțiuni.{Environment.NewLine}{Environment.NewLine}" +
                        $"    <B>Perioada:</B> selectați o perioadă pentru afișarea camerelor libere în perioada respectiva.{Environment.NewLine}" +
                        $"    <B>Tipul de cameră:</B> selecția tipului va afișa doar camerele respective.{Environment.NewLine}" +
                        $"    <B>Numărul de paturi:</B> selectați un anumit numar de paturi pentru a afișa doar camerele cu configurația respectivă.{Environment.NewLine}" +
                        $"    <B>Client:</B> alegeți un client pentru a afișa camerele cu rezervări făcute pe clientul respectiv.{Environment.NewLine}{Environment.NewLine}" +
                        $"    Filtrele se pot folosi infividual sau împreună.{Environment.NewLine}{Environment.NewLine}" +
                        $"    Pentru a șterge filtrele active, apăsați pe icoana <sprite name=\"xfilter\" tint=1> din stânga calendarului din pagina principală";
                break;
            default:
                break;
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentTransform);
        Canvas.ForceUpdateCanvases();
    }
}
