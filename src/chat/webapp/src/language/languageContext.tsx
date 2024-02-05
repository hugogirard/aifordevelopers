import { i18n } from "i18next";
import { defaultLanguage } from "../i18next";
import { createContext, useContext } from "react";
import { useTranslation } from "react-i18next";
import { RootState } from '../redux/app/store';
import { useAppDispatch, useAppSelector } from "../redux/app/hooks";
import { IChatMessage } from "../libs/models/ChatMessage";
import { AuthHelper } from "../libs/auth/AuthHelper";
import { useMsal } from "@azure/msal-react";
import { updateMessageProperty } from '../redux/features/conversations/conversationsSlice';

const languages = {
  fr: { nativeName: "Français" },
  en: { nativeName: "English" },  
};

const defaultContext = {
  t: (key: string) => key,
  i18n: null as unknown as i18n,
  onClickLanguageChange: (e: any) => { e==e; },
  languages: languages
};

export let currentLanguage = defaultLanguage;
export let translationFunc = (key: string) => key;
export const LanguageContext = createContext(defaultContext);

interface ITranslationResult {
  translations: ITranslation[];
}

interface ITranslation {
  text: string;
  to: string
}

interface IAccessToken {
  token: string;
  region: string;
  isSuccess: boolean;
}

export const LanguageContextProvider = ({ children }: { children: React.ReactNode }) => {

  const { t, i18n } = useTranslation();
  translationFunc = t;
  const dispatch = useAppDispatch();
  const { instance, inProgress } = useMsal();
  const { conversations } = useAppSelector((state: RootState) => state.conversations);

  const BackendServiceUrl =
  process.env.REACT_APP_BACKEND_URI == null || process.env.REACT_APP_BACKEND_URI.trim() === ''
      ? window.origin
      : process.env.REACT_APP_BACKEND_URI;
    
  const onClickLanguageChange = (language: string) => {
    onClickLanguageChangeAsync(language).catch((r) => { throw r; })
  }

  const onClickLanguageChangeAsync = async (language: string) => {

    const sourceLang = currentLanguage;
    const targetLang = language;

    const translationJson: any[] = [];
    const messagesToTranslate: IChatMessage[] = [];
    Object.keys(conversations).forEach((convoKey) => { 
      conversations[convoKey].messages.forEach((message) => {
        const key = Object.keys(message.translations).find(key => key == targetLang);
        if (key) 
        {
          dispatch(
            updateMessageProperty({
              chatId: convoKey,
              messageIdOrIndex: message.id as string,
              property: 'content',
              value: message.translations[key],
              frontLoad: true,
            })
          );
        }
        else
        {
          messagesToTranslate.push(message);
          translationJson.push({ "text": message.content });
        }
      });
    });

    if (messagesToTranslate.length > 0) {
      const tokenUri = new URL("speechToken", BackendServiceUrl);
      const accessTokenResponse = await fetch(tokenUri, {
          method: "GET",                    
          headers: {
              "Authorization": `Bearer ${await AuthHelper.getSKaaSAccessToken(instance, inProgress)}`,
              "Content-type": "application/json"
          }
        });      
      const accessToken = await accessTokenResponse.json() as IAccessToken;
      const translationResponse = await fetch(`https://api.cognitive.microsofttranslator.com/translate?api-version=3.0&from=${sourceLang}&to=${targetLang}`, {
        method: "POST",                    
        headers: {
            "Authorization": `Bearer ${accessToken.token}`,
            "Content-type": "application/json"
        },
        body: JSON.stringify(translationJson)
      });      
      const translationResults = await translationResponse.json() as ITranslationResult[];      

      for (let i = 0; i < translationResults.length; i++) {
        dispatch(
          updateMessageProperty({
            chatId: messagesToTranslate[i].chatId,
            messageIdOrIndex: messagesToTranslate[i].id as string,
            property: 'content',
            value: translationResults[i].translations[0].text,
            frontLoad: true,
          })
        );
       
        dispatch(
          updateMessageProperty({
            chatId: messagesToTranslate[i].chatId,
            messageIdOrIndex: messagesToTranslate[i].id as string,
            property: 'translations',
            value:  {...messagesToTranslate[i].translations, [targetLang]: translationResults[i].translations[0].text},
            frontLoad: true,
          })
        );
      }
    }

    currentLanguage = targetLang;
    await i18n.changeLanguage(targetLang);
  };

  return (
    <LanguageContext.Provider
      value={{ t, i18n, onClickLanguageChange, languages }}
    >
      {children}
    </LanguageContext.Provider>
  );
};

export const useLanguageContext = () => useContext(LanguageContext);