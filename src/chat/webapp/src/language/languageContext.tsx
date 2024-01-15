import { i18n } from "i18next";
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

export let CurrentLanguage = "fr";
export const LanguageContext = createContext(defaultContext);

export const LanguageContextProvider = ({ children }: { children: React.ReactNode }) => {

  const { t, i18n } = useTranslation();

  const onClickLanguageChange = (e: { target: { value: string } }) => {
    const language = e.target.value;
    CurrentLanguage = language;
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