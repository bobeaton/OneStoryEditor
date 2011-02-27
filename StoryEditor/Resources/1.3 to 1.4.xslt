<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt" exclude-result-prefixes="msxsl"
>
  <xsl:output method="xml" indent="yes"/>

  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!--StoryProject@version changed from 1.3 to 1.4-->
  <xsl:template match="StoryProject/@version">
    <xsl:attribute name="version">
      <xsl:text>1.4</xsl:text>
    </xsl:attribute>
  </xsl:template>
  
  <!--Set the 'use' parameters based on the older configuration-->
  <xsl:template match="Languages">
    <Languages UseRetellingVernacular="false" UseRetellingNationalBT="false" UseRetellingInternationalBT="true" UseAnswerVernacular="false" UseAnswerNationalBT="false" UseAnswerInternationalBT="true" UseTestQuestionInternationalBT="true">
      <xsl:attribute name="UseTestQuestionVernacular">
        <xsl:choose>
          <xsl:when test="//stories/story/verses/verse/TestQuestions/TestQuestion/TQVernacular">
            <xsl:text>true</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>false</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:attribute name="UseTestQuestionNationalBT">
        <xsl:choose>
          <xsl:when test="//stories/story/verses/verse/TestQuestions/TestQuestion/TQNationalBT">
            <xsl:text>true</xsl:text>
          </xsl:when>
          <xsl:otherwise>
            <xsl:text>false</xsl:text>
          </xsl:otherwise>
        </xsl:choose>
      </xsl:attribute>
      <xsl:apply-templates select="@*|node()"/>
    </Languages>
  </xsl:template>
  
  <!--VernacularLang was changed to LanguageInfo + @lang="Vernacular"-->
  <xsl:template match="VernacularLang">
    <LanguageInfo lang="Vernacular">
      <xsl:apply-templates select="@*|node()"/>
    </LanguageInfo>
  </xsl:template>

  <!--NationalBTLang was changed to LanguageInfo + @lang="NationalBt"-->
  <xsl:template match="NationalBTLang">
    <LanguageInfo lang="NationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </LanguageInfo>
  </xsl:template>

  <!--InternationalBTLang was changed to LanguageInfo + @lang="InternationalBt"-->
  <xsl:template match="InternationalBTLang">
    <LanguageInfo lang="InternationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </LanguageInfo>
  </xsl:template>

  <!--verses was changed to Verses-->
  <xsl:template match="verses">
    <Verses>
      <xsl:apply-templates select="@*|node()"/>
    </Verses>
  </xsl:template>

  <!--verse was changed to Verse-->
  <xsl:template match="verse">
    <Verse>
      <xsl:apply-templates select="@*|node()"/>
    </Verse>
  </xsl:template>

  <!--anchors was changed to Anchors-->
  <xsl:template match="anchors">
    <Anchors>
      <xsl:apply-templates select="@*|anchor"/>
    </Anchors >
    <xsl:if test="anchor/exegeticalHelps">
      <ExegeticalHelps>
        <xsl:for-each select="anchor/exegeticalHelps/exegeticalHelp">
          <ExegeticalHelp>
            <xsl:value-of select="text()"/>
          </ExegeticalHelp>
        </xsl:for-each>
      </ExegeticalHelps>
    </xsl:if>
    <!--<xsl:apply-templates select="anchor/exegeticalHelps"/>-->
  </xsl:template>

  <!--anchor was changed to Anchor-->
  <xsl:template match="anchor">
    <Anchor>
      <xsl:apply-templates select="@*"/>
      <xsl:value-of select="toolTip/text()"/>
    </Anchor >
  </xsl:template>

  <xsl:template match="exegeticalHelps">
    <ExegeticalHelps>
      <xsl:for-each select="exegeticalHelp">
        <ExegeticalHelp>
          <xsl:value-of select="text()"/>
        </ExegeticalHelp>
      </xsl:for-each>
    </ExegeticalHelps>
  </xsl:template>
  
  <!--Vernacular was changed to StoryLine + @lang="Vernacular"-->
  <xsl:template match="Vernacular">
    <StoryLine lang="Vernacular">
      <xsl:apply-templates select="@*|node()"/>
    </StoryLine>
  </xsl:template>

  <!--NationalBT was changed to StoryLine + @lang="NationalBt"-->
  <xsl:template match="NationalBT">
    <StoryLine lang="NationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </StoryLine>
  </xsl:template>

  <!--InternationalBT was changed to StoryLine + @lang="InternationalBt"-->
  <xsl:template match="InternationalBT">
    <StoryLine lang="InternationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </StoryLine>
  </xsl:template>

  <!--FreeTranslation was changed to StoryLine + @lang="FreeTranslation"-->
  <xsl:template match="FreeTranslation">
    <StoryLine lang="FreeTranslation">
      <xsl:apply-templates select="@*|node()"/>
    </StoryLine>
  </xsl:template>

  <!--TQVernacular was changed to TestQuestionLine + @lang="Vernacular"-->
  <xsl:template match="TQVernacular">
    <TestQuestionLine lang="Vernacular">
      <xsl:apply-templates select="@*|node()"/>
    </TestQuestionLine>
  </xsl:template>

  <!--TQNationalBT was changed to TestQuestionLine + @lang="NationalBt"-->
  <xsl:template match="TQNationalBT">
    <TestQuestionLine lang="NationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </TestQuestionLine>
  </xsl:template>

  <!--TQInternationalBT was changed to TestQuestionLine + @lang="InternationalBt"-->
  <xsl:template match="TQInternationalBT">
    <TestQuestionLine lang="InternationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </TestQuestionLine>
  </xsl:template>

  <!--answer was changed to Answer + @lang="InternationalBt"-->
  <xsl:template match="answer">
    <Answer lang="InternationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </Answer>
  </xsl:template>

  <!--Retelling was changed to Retelling + @lang="InternationalBt"-->
  <xsl:template match="Retelling">
    <Retelling lang="InternationalBt">
      <xsl:apply-templates select="@*|node()"/>
    </Retelling>
  </xsl:template>

</xsl:stylesheet>
