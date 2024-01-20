import { i18n } from "i18next";
import { defaultLanguage } from "../i18next";
import { createContext, useContext } from "react";
import { useTranslation } from "react-i18next";

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

export const LanguageContextProvider = ({ children }: { children: React.ReactNode }) => {

  const { t, i18n } = useTranslation();
  translationFunc = t;

  const onClickLanguageChange = (language: string) => {
    currentLanguage = language;
    i18n.changeLanguage(language).catch(() => { console.error("Error changing language"); });; //change the language
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