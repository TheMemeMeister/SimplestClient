using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameSystemManager : MonoBehaviour
{

    GameObject submitButton, userNameInput, passowrdInput, createToggle, loginToggle;
    GameObject networkedClient;




    void Start()
    {
        GameObject[] allObject = UnityEngine.Object.FindObjectsOfType<GameObject>();
        foreach (GameObject go in allObject)
        {
            if (go.name == "UserName Input")
            {
                userNameInput = go;
            }
            else if (go.name == "Password Input")
            {
                passowrdInput = go;
            }
            else if (go.name == "SubmitButton")
            {
                submitButton = go;
            }
            else if (go.name == "LoginToggle")
            {
                loginToggle = go;
            }
            else if (go.name == "CreateToggle")
            {
                createToggle = go;
            }
            else if (go.name == "NetworkedClient")
            {
                networkedClient = go;
            }
        }
        submitButton.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        loginToggle.GetComponent<Toggle>().onValueChanged.AddListener(LoginToggleChanged);
        createToggle.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void SubmitButtonPressed()
    {
        //send login stuff to server
        string p = passowrdInput.GetComponent<InputField>().text;
        string n = userNameInput.GetComponent<InputField>().text;
        string msg;

        if(createToggle.GetComponent<Toggle>().isOn)
        {
            msg = ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        }
        else
            msg = ClientToServerSignifiers.LoginAccount + "," + n + "," + p;

        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

      
        Debug.Log(msg);

    }
    public void LoginToggleChanged(bool loginbool)
    {
        Debug.Log("hey");
        loginToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!loginbool);
    }
    public void CreateToggleChanged(bool createbool)
    {
        Debug.Log("Nay");
        createToggle.GetComponent<Toggle>().SetIsOnWithoutNotify(!createbool);
    }
}
